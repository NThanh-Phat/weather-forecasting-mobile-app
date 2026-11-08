
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'package:vietweather_ai/main.dart';

void main() {
  testWidgets('VietWeatherApp renders main screen', (WidgetTester tester) async {
    await tester.pumpWidget(const VietWeatherApp());

    // Kiểm tra ứng dụng được khởi tạo
    expect(find.byType(MaterialApp), findsOneWidget);

    // Kiểm tra màn hình chính
    expect(find.byType(MainScreen), findsOneWidget);
  });
}
