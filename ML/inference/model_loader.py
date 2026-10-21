
from pathlib import Path
import joblib

from tensorflow.keras.models import load_model

from config.config import (
    MODEL_PATH,
    SCALER_PATH,
    WINDOW_SIZE,
    EXPECTED_FEATURES,
)


def load_gru_and_scaler():
    if not MODEL_PATH.exists():
        raise FileNotFoundError(
            f"Không tìm thấy mô hình GRU: {MODEL_PATH}"
        )

    if not SCALER_PATH.exists():
        raise FileNotFoundError(
            f"Không tìm thấy scaler: {SCALER_PATH}"
        )

    # Tải mô hình GRU đã huấn luyện
    model = load_model(
        MODEL_PATH,
        compile=False,
    )

    # Đọc scaler bằng Joblib
    scaler = joblib.load(SCALER_PATH)

    # Kiểm tra input model
    expected_input = (
        WINDOW_SIZE,
        EXPECTED_FEATURES,
    )

    actual_input = tuple(model.input_shape[1:])

    if actual_input != expected_input:
        raise ValueError(
            f"Input GRU: {actual_input}; "
            f"mong đợi {expected_input}."
        )

    # Kiểm tra scaler
    scaler_features = getattr(
        scaler,
        "n_features_in_",
        None,
    )

    if scaler_features != EXPECTED_FEATURES:
        raise ValueError(
            f"Scaler có {scaler_features} features, "
            f"mong đợi {EXPECTED_FEATURES}."
        )

    print("Đã tải mô hình GRU.")
    print("Model input:", model.input_shape)
    print("Model output:", model.output_shape)
    print("Scaler features:", scaler_features)

    return model, scaler
