
import numpy as np
import pandas as pd

from config.config import (
    FEATURE_COLUMNS,
    LAGS,
    WEATHER_LAG_COLUMNS,
)
from preprocessing.humidity import (
    relative_to_specific_humidity,
)


def build_features(raw_df: pd.DataFrame) -> pd.DataFrame:
    df = raw_df.copy()

    df["datetime"] = pd.to_datetime(df["datetime"])
    df = (
        df.sort_values("datetime")
        .drop_duplicates("datetime")
        .reset_index(drop=True)
    )

    # Đảm bảo dữ liệu liên tục theo từng giờ
    gaps = df["datetime"].diff().dropna()

    if not (gaps == pd.Timedelta(hours=1)).all():
        raise ValueError(
            "Dữ liệu không liên tục theo từng giờ."
        )

    # 1. Humidity: chuyển RH sang Specific Humidity
    df["Humidity"] = relative_to_specific_humidity(
        df["Temperature"],
        df["Relative_Humidity"],
        df["Surface_Pressure"],
    )

    # 2. Wind Speed: km/h -> m/s
    df["Wind_Speed"] = df["Wind_Speed_kmh"] / 3.6

    # 3. Cloud Cover: % -> tỷ lệ 0..1
    df["Cloud_Cover"] = (
        df["Cloud_Cover_Percent"] / 100.0
    )

    # 4. Time features
    df["Hour"] = df["datetime"].dt.hour
    df["Day"] = df["datetime"].dt.day
    df["Month"] = df["datetime"].dt.month

    df["Hour_Sin"] = np.sin(
        2 * np.pi * df["Hour"] / 24
    )
    df["Hour_Cos"] = np.cos(
        2 * np.pi * df["Hour"] / 24
    )

    df["Month_Sin"] = np.sin(
        2 * np.pi * df["Month"] / 12
    )
    df["Month_Cos"] = np.cos(
        2 * np.pi * df["Month"] / 12
    )

    # 5. Lag features
    for column in WEATHER_LAG_COLUMNS:
        for lag in LAGS:
            df[f"{column}_lag_{lag}"] = (
                df[column].shift(lag)
            )

    # Lấy đúng 54 cột theo thứ tự cấu hình
    feature_df = df[FEATURE_COLUMNS].copy()

    feature_df = feature_df.replace(
        [np.inf, -np.inf],
        np.nan,
    )

    # Giữ lại datetime của những dòng hợp lệ
    valid_mask = feature_df.notna().all(axis=1)
    valid_datetimes = (
        df.loc[valid_mask, "datetime"]
        .reset_index(drop=True)
    )

    feature_df = (
        feature_df.loc[valid_mask]
        .reset_index(drop=True)
    )

    if feature_df.shape[1] != 54:
        raise ValueError(
            f"Số feature hiện tại là {feature_df.shape[1]}, "
            "yêu cầu 54."
        )

    if len(feature_df) < 24:
        raise ValueError(
            f"Chỉ còn {len(feature_df)} dòng hợp lệ. "
            "Cần tối thiểu 24 dòng feature sau khi tạo lag."
        )

    feature_df.insert(0, "datetime", valid_datetimes)

    return feature_df
