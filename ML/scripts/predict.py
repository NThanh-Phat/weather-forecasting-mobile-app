
import argparse
import sys
from pathlib import Path

# Cho phép import các module trong project ML
PROJECT_ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT_ROOT))

from inference.predictor import predict_next_24_hours


def main():
    parser = argparse.ArgumentParser(
        description="Dự báo nhiệt độ 24 giờ bằng GRU."
    )

    parser.add_argument(
        "--lat",
        type=float,
        default=None,
        help="Vĩ độ địa điểm",
    )

    parser.add_argument(
        "--lon",
        type=float,
        default=None,
        help="Kinh độ địa điểm",
    )

    parser.add_argument(
        "--output",
        default="forecast_gru_24h.csv",
        help="Tên file CSV đầu ra",
    )

    args = parser.parse_args()

    result = predict_next_24_hours(
        latitude=args.lat,
        longitude=args.lon,
    )

    output_path = Path(args.output)

    result.to_csv(
        output_path,
        index=False,
        encoding="utf-8-sig",
    )

    print("\nKẾT QUẢ DỰ BÁO GRU")
    print(result.to_string(index=False))
    print(f"\nĐã lưu file: {output_path.resolve()}")


if __name__ == "__main__":
    main()
