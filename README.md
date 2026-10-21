# weather-forecasting-mobile-app

# ML - Đồ án 4: Dự báo nhiệt độ bằng GRU

## 1. Mục tiêu

Sử dụng mô hình GRU đã huấn luyện trong Đồ án 3
để dự báo nhiệt độ 24 giờ tiếp theo.

Không huấn luyện lại mô hình trong Đồ án 4.

## 2. Thành phần

- Python và TensorFlow/Keras
- Open-Meteo API
- pandas và NumPy
- Scikit-learn scaler

## 3. Mô hình và scaler

Đặt các file gốc tại:

- models/GRU.keras
- scalers/scaler.pkl

Không thay file GRU bằng LSTM đổi tên.
Không fit lại scaler bằng dữ liệu Open-Meteo.

## 4. Chạy dự báo

Cài đặt thư viện:

    pip install -r requirements.txt

Chạy:

    python scripts/predict.py

Kết quả mặc định được lưu tại:

    forecast_gru_24h.csv

## 5. Kiểm thử

    python -m unittest discover -s tests -v

## 6. Trước khi tích hợp backend

Cần xác minh:

1. Input shape của GRU.
2. Output shape của GRU.
3. Đúng scaler đã dùng khi huấn luyện GRU.
4. Thứ tự chính xác của 54 features.
5. Đơn vị của Humidity và Wind_Speed.
6. Đầu ra mục tiêu có phải nhiệt độ °C chưa chuẩn hóa hay không.
