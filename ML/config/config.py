
from pathlib import Path

# Thư mục gốc ML
BASE_DIR = Path(__file__).resolve().parents[1]

# Đường dẫn model và scaler đã huấn luyện ở Đồ án 3
MODEL_PATH = BASE_DIR / "models" / "GRU.keras"
SCALER_PATH = BASE_DIR / "scalers" / "scaler.pkl"

# Tọa độ mặc định: Cần Thơ
LATITUDE = 10.0452
LONGITUDE = 105.7469
TIMEZONE = "Asia/Bangkok"

# Cấu hình đầu vào/đầu ra
WINDOW_SIZE = 24
EXPECTED_FEATURES = 54
FORECAST_HOURS = 24

# Các khoảng lag tính theo giờ
LAGS = [1, 3, 6, 12, 24, 48, 168]

# Sáu biến dùng tạo lag
WEATHER_LAG_COLUMNS = [
    "Temperature",
    "Humidity",
    "Wind_Speed",
    "Precipitation",
    "Cloud_Cover",
    "Irradiance_Surface",
]

# 5 đặc trưng thời tiết đầu vào hiện tại
WEATHER_FEATURE_COLUMNS = [
    "Humidity",
    "Wind_Speed",
    "Precipitation",
    "Cloud_Cover",
    "Irradiance_Surface",
]

# 7 đặc trưng thời gian
TIME_FEATURE_COLUMNS = [
    "Hour",
    "Day",
    "Month",
    "Hour_Sin",
    "Hour_Cos",
    "Month_Sin",
    "Month_Cos",
]

# Thứ tự feature: phải đối chiếu với pipeline tạo dữ liệu train GRU
FEATURE_COLUMNS = (
    WEATHER_FEATURE_COLUMNS
    + TIME_FEATURE_COLUMNS
    + [
        f"{column}_lag_{lag}"
        for column in WEATHER_LAG_COLUMNS
        for lag in LAGS
    ]
)

# Lấy lịch sử và dữ liệu dự báo từ Open-Meteo
PAST_DAYS = 8
FORECAST_DAYS = 1
