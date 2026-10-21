
import numpy as np


def relative_to_specific_humidity(
    temperature_c,
    relative_humidity_percent,
    surface_pressure_hpa,
):
    """
    Tính Specific Humidity q theo công thức gần đúng.

    Đầu ra hiện tại: kg/kg.
    Phải xác minh đơn vị Humidity của dữ liệu train GRU
    trước khi sử dụng scaler.
    """

    temperature = np.asarray(temperature_c, dtype=float)
    rh = np.asarray(relative_humidity_percent, dtype=float) / 100.0
    pressure = np.asarray(surface_pressure_hpa, dtype=float)

    rh = np.clip(rh, 0.0, 1.0)

    # Áp suất hơi nước bão hòa (hPa)
    saturation_vapor_pressure = (
        6.112
        * np.exp(
            (17.67 * temperature)
            / (temperature + 243.5)
        )
    )

    # Áp suất hơi nước thực tế (hPa)
    vapor_pressure = rh * saturation_vapor_pressure

    epsilon = 0.622

    denominator = (
        pressure
        - (1.0 - epsilon) * vapor_pressure
    )

    if np.any(denominator <= 0):
        raise ValueError(
            "Áp suất không hợp lệ khi tính Specific Humidity."
        )

    specific_humidity = (
        epsilon * vapor_pressure / denominator
    )

    return specific_humidity
