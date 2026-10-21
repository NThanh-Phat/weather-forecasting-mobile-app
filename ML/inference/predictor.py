
import numpy as np
import pandas as pd

from datetime import datetime, timedelta
from zoneinfo import ZoneInfo

from config.config import (
    WINDOW_SIZE,
    EXPECTED_FEATURES,
    FORECAST_HOURS,
    FEATURE_COLUMNS,
    TIMEZONE,
)

from preprocessing.openmeteo_client import fetch_weather
from preprocessing.feature_engineering import build_features
from inference.model_loader import load_gru_and_scaler


def predict_next_24_hours(latitude=None, longitude=None):
    # 1. Lấy dữ liệu Open-Meteo
    api_args = {}

    if latitude is not None:
        api_args["latitude"] = latitude

    if longitude is not None:
        api_args["longitude"] = longitude

    raw_df = fetch_weather(**api_args)

    if raw_df.empty:
        raise ValueError("Open-Meteo không trả về dữ liệu.")

    # 2. Chuẩn hóa thời gian và loại bản ghi trùng
    raw_df["datetime"] = pd.to_datetime(raw_df["datetime"])

    raw_df = (
        raw_df.sort_values("datetime")
        .drop_duplicates(subset="datetime")
        .reset_index(drop=True)
    )

    now = datetime.now(ZoneInfo(TIMEZONE))

    # Chỉ giữ dữ liệu đến đầu giờ hiện tại.
    # Ví dụ 09:38 thì giữ dữ liệu đến 09:00.
    current_hour = now.replace(
        minute=0,
        second=0,
        microsecond=0,
        tzinfo=None,
    )

    raw_df = raw_df.loc[
        raw_df["datetime"] <= current_hour
    ].copy().reset_index(drop=True)

    if raw_df.empty:
        raise ValueError(
            "Không có dữ liệu Open-Meteo đến giờ hiện tại."
        )

    latest_time = raw_df["datetime"].max()

    print("Thời điểm chạy:", now.strftime("%Y-%m-%d %H:%M"))
    print("Dữ liệu lịch sử bắt đầu:", raw_df["datetime"].min())
    print("Dữ liệu lịch sử kết thúc:", latest_time)
    print("Số dòng dữ liệu API sau lọc:", len(raw_df))

    # 3. Kiểm tra dữ liệu theo giờ
    gaps = raw_df["datetime"].diff().dropna()

    if not (gaps == pd.Timedelta(hours=1)).all():
        raise ValueError(
            "Dữ liệu lịch sử không liên tục theo giờ. "
            "Cần xử lý khoảng trống trước khi dự báo."
        )

    # 4. Tạo 54 features và các lag
    feature_df = build_features(raw_df)

    if feature_df.empty:
        raise ValueError("Không tạo được feature hợp lệ.")

    # Chỉ lấy các dòng có đủ 54 feature
    missing_columns = [
        col for col in FEATURE_COLUMNS
        if col not in feature_df.columns
    ]

    if missing_columns:
        raise ValueError(
            f"Thiếu feature: {missing_columns}"
        )

    X_features = feature_df.loc[
        :, FEATURE_COLUMNS
    ].copy()

    X_features = X_features.replace(
        [np.inf, -np.inf], np.nan
    )

    valid_mask = X_features.notna().all(axis=1)

    X_features = X_features.loc[valid_mask].copy()

    if len(X_features) < WINDOW_SIZE:
        raise ValueError(
            f"Chỉ có {len(X_features)} dòng feature hợp lệ; "
            f"cần ít nhất {WINDOW_SIZE} dòng."
        )

    # 5. Tải GRU và scaler gốc
    model, scaler = load_gru_and_scaler()

    if scaler.n_features_in_ != EXPECTED_FEATURES:
        raise ValueError(
            f"Scaler yêu cầu {scaler.n_features_in_} features, "
            f"nhưng cấu hình là {EXPECTED_FEATURES}."
        )

    # Xác minh thứ tự cột nếu scaler có lưu tên feature
    if hasattr(scaler, "feature_names_in_"):
        scaler_features = list(scaler.feature_names_in_)

        if list(FEATURE_COLUMNS) != scaler_features:
            mismatches = [
                (i + 1, actual, expected)
                for i, (actual, expected) in enumerate(
                    zip(FEATURE_COLUMNS, scaler_features)
                )
                if actual != expected
            ]

            raise ValueError(
                "Thứ tự features không khớp scaler. "
                f"Các vị trí khác nhau: {mismatches}"
            )

    # Giữ DataFrame để scaler nhận tên cột.
    # Không fit lại scaler.
    X_scaled = scaler.transform(X_features)

    # 6. Lấy 24 dòng feature hợp lệ mới nhất
    X = X_scaled[-WINDOW_SIZE:]
    X = np.expand_dims(X, axis=0)

    expected_shape = (
        1,
        WINDOW_SIZE,
        EXPECTED_FEATURES,
    )

    if X.shape != expected_shape:
        raise ValueError(
            f"Input shape sai: {X.shape}; "
            f"yêu cầu {expected_shape}."
        )

    # 7. Dự báo bằng GRU
    prediction = np.asarray(
        model.predict(X, verbose=0)
    )

    if prediction.shape == (1, FORECAST_HOURS):
        prediction = prediction[0]

    elif prediction.shape == (1, FORECAST_HOURS, 1):
        prediction = prediction[0, :, 0]

    else:
        raise ValueError(
            f"Output GRU không đúng dạng "
            f"{FORECAST_HOURS} giờ: {prediction.shape}."
        )

    # 8. Gắn thời gian theo dữ liệu thực sự được dùng
    # Lấy timestamp cuối cùng tương ứng với dòng feature cuối.
    valid_feature_rows = feature_df.loc[valid_mask]

    latest_feature_time = pd.to_datetime(
        valid_feature_rows["datetime"].iloc[-1]
    )

    forecast_start = latest_feature_time + timedelta(hours=1)

    forecast_times = pd.date_range(
        start=forecast_start,
        periods=FORECAST_HOURS,
        freq="h",
    )

    result = pd.DataFrame({
        "datetime": forecast_times,
        "Predicted_Temperature_C": prediction.astype(float),
    })

    print("Số dòng feature hợp lệ:", len(X_features))
    print("Số feature:", len(FEATURE_COLUMNS))
    print("Input GRU:", X.shape)
    print("Output GRU:", prediction.shape)
    print("Bản ghi cuối được dùng:", latest_feature_time)
    print("Bắt đầu dự báo:", forecast_times[0])
    print("Kết thúc dự báo:", forecast_times[-1])

    return result
