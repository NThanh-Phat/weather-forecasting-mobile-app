
import requests
import pandas as pd

from config.config import (
    LATITUDE,
    LONGITUDE,
    TIMEZONE,
    PAST_DAYS,
    FORECAST_DAYS,
)

OPEN_METEO_URL = "https://api.open-meteo.com/v1/forecast"


def fetch_weather(
    latitude=LATITUDE,
    longitude=LONGITUDE,
    past_days=PAST_DAYS,
    forecast_days=FORECAST_DAYS,
):
    params = {
        "latitude": latitude,
        "longitude": longitude,
        "hourly": [
            "temperature_2m",
            "relative_humidity_2m",
            "surface_pressure",
            "precipitation",
            "cloud_cover",
            "wind_speed_10m",
            "shortwave_radiation",
        ],
        "past_days": past_days,
        "forecast_days": forecast_days,
        "timezone": TIMEZONE,
        "wind_speed_unit": "kmh",
        "precipitation_unit": "mm",
        "temperature_unit": "celsius",
    }

    response = requests.get(
        OPEN_METEO_URL,
        params=params,
        timeout=30,
    )
    response.raise_for_status()

    data = response.json()
    hourly = data.get("hourly")

    if not hourly or "time" not in hourly:
        raise ValueError(
            "Open-Meteo không trả về dữ liệu theo giờ."
        )

    df = pd.DataFrame({
        "datetime": pd.to_datetime(hourly["time"]),
        "Temperature": hourly["temperature_2m"],
        "Relative_Humidity": hourly["relative_humidity_2m"],
        "Surface_Pressure": hourly["surface_pressure"],
        "Precipitation": hourly["precipitation"],
        "Cloud_Cover_Percent": hourly["cloud_cover"],
        "Wind_Speed_kmh": hourly["wind_speed_10m"],
        "Irradiance_Surface": hourly["shortwave_radiation"],
    })

    df = (
        df.sort_values("datetime")
        .drop_duplicates("datetime")
        .reset_index(drop=True)
    )

    if df.isna().any().any():
        raise ValueError(
            "Dữ liệu Open-Meteo có giá trị thiếu. "
            "Hãy kiểm tra trước khi dự báo."
        )

    print("Số dòng dữ liệu:", len(df))
    print("Thời gian bắt đầu:", df["datetime"].min())
    print("Thời gian kết thúc:", df["datetime"].max())

    return df
