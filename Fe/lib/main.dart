import 'package:flutter/material.dart';

void main() {
  runApp(const VietWeatherApp());
}

// ==========================================
// 1. CẤU HÌNH ỨNG DỤNG
// ==========================================

class AppColors {
  static const primary = Color(0xFF0284C7);
  static const darkBlue = Color(0xFF172554);
  static const background = Color(0xFFF0F7FF);
  static const lightBlue = Color(0xFFDBEAFE);
  static const white = Colors.white;
  static const text = Color(0xFF1E293B);
  static const grey = Color(0xFF64748B);
}

class VietWeatherApp extends StatelessWidget {
  const VietWeatherApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'VietWeather AI',
      theme: ThemeData(
        useMaterial3: true,
        scaffoldBackgroundColor: AppColors.background,
        colorScheme: ColorScheme.fromSeed(seedColor: AppColors.primary),
        fontFamily: 'Roboto',
      ),
      home: const MainScreen(),
    );
  }
}

// ==========================================
// 2. MÀN HÌNH CHÍNH VÀ THANH ĐIỀU HƯỚNG
// ==========================================

class MainScreen extends StatefulWidget {
  const MainScreen({super.key});

  @override
  State<MainScreen> createState() => _MainScreenState();
}

class _MainScreenState extends State<MainScreen> {
  int selectedIndex = 0;

  final List<Widget> screens = const [
    WeatherScreen(),
    LocationScreen(),
    SettingsScreen(),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: IndexedStack(index: selectedIndex, children: screens),
      bottomNavigationBar: NavigationBar(
        selectedIndex: selectedIndex,
        backgroundColor: Colors.white,
        indicatorColor: const Color(0xFFE0F2FE),
        onDestinationSelected: (index) {
          setState(() {
            selectedIndex = index;
          });
        },
        destinations: const [
          NavigationDestination(
            icon: Icon(Icons.wb_sunny_outlined),
            selectedIcon: Icon(Icons.wb_sunny),
            label: 'Thời tiết',
          ),
          NavigationDestination(
            icon: Icon(Icons.location_on_outlined),
            selectedIcon: Icon(Icons.location_on),
            label: 'Địa điểm',
          ),
          NavigationDestination(
            icon: Icon(Icons.settings_outlined),
            selectedIcon: Icon(Icons.settings),
            label: 'Cài đặt',
          ),
        ],
      ),
    );
  }
}

// ==========================================
// 3. MÀN HÌNH TỔNG QUAN THỜI TIẾT
// ==========================================

class WeatherScreen extends StatelessWidget {
  const WeatherScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: SingleChildScrollView(
        padding: const EdgeInsets.fromLTRB(16, 12, 16, 24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // HEADER
            Row(
              children: [
                const CircleAvatar(
                  radius: 19,
                  backgroundColor: AppColors.lightBlue,
                  child: Icon(Icons.cloud, color: AppColors.primary),
                ),
                const SizedBox(width: 9),
                const Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'VIETWEATHER AI',
                        style: TextStyle(
                          color: AppColors.primary,
                          fontSize: 10,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Text(
                        'Thời Tiết',
                        style: TextStyle(
                          color: AppColors.darkBlue,
                          fontSize: 15,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 12,
                    vertical: 8,
                  ),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(20),
                  ),
                  child: const Row(
                    children: [
                      Icon(
                        Icons.location_on,
                        size: 14,
                        color: AppColors.primary,
                      ),
                      SizedBox(width: 4),
                      Text('Cần Thơ', style: TextStyle(fontSize: 12)),
                      Icon(Icons.keyboard_arrow_down, size: 16),
                    ],
                  ),
                ),
              ],
            ),

            const SizedBox(height: 22),
            const SizedBox(height: 14),

            // THANH TÌM KIẾM
            TextField(
              decoration: InputDecoration(
                hintText: 'Tìm kiếm tỉnh, thành phố...',
                hintStyle: const TextStyle(
                  fontSize: 12,
                  color: Color(0xFF94A3B8),
                ),
                prefixIcon: const Icon(
                  Icons.search,
                  color: AppColors.primary,
                  size: 20,
                ),
                suffixIcon: const Icon(
                  Icons.mic_none,
                  color: AppColors.grey,
                  size: 19,
                ),
                filled: true,
                fillColor: Colors.white,
                contentPadding: const EdgeInsets.symmetric(vertical: 12),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(14),
                  borderSide: BorderSide.none,
                ),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(14),
                  borderSide: BorderSide.none,
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(14),
                  borderSide: const BorderSide(
                    color: AppColors.primary,
                    width: 1,
                  ),
                ),
              ),
            ),

            const SizedBox(height: 18),

            // TIÊU ĐỀ
            const Text(
              'TỔNG QUAN THỜI TIẾT',
              style: TextStyle(
                color: AppColors.primary,
                fontSize: 12,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 4),
            const Text(
              'Thời tiết hôm nay tại Cần Thơ',
              style: TextStyle(color: AppColors.grey, fontSize: 12),
            ),

            const SizedBox(height: 16),

            // THỜI TIẾT HIỆN TẠI
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(18),
              decoration: BoxDecoration(
                gradient: const LinearGradient(
                  colors: [Colors.white, Color(0xFFE0F2FE)],
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                ),
                borderRadius: BorderRadius.circular(20),
                boxShadow: [
                  BoxShadow(
                    color: Colors.blue.withValues(alpha: 0.07),
                    blurRadius: 12,
                    offset: const Offset(0, 4),
                  ),
                ],
              ),
              child: Column(
                children: [
                  const Row(
                    children: [
                      Icon(
                        Icons.location_on,
                        color: AppColors.primary,
                        size: 15,
                      ),
                      SizedBox(width: 5),
                      Text(
                        'TP. Cần Thơ',
                        style: TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.bold,
                          color: AppColors.darkBlue,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 12),

                  Row(
                    children: [
                      const Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              '29°',
                              style: TextStyle(
                                fontSize: 58,
                                fontWeight: FontWeight.w300,
                                color: AppColors.darkBlue,
                              ),
                            ),
                            Text(
                              '☀  Mây rải rác',
                              style: TextStyle(
                                fontSize: 13,
                                color: AppColors.grey,
                              ),
                            ),
                            SizedBox(height: 5),
                            Text(
                              'Cảm giác như 32°C',
                              style: TextStyle(
                                fontSize: 11,
                                color: AppColors.grey,
                              ),
                            ),
                          ],
                        ),
                      ),

                      Container(
                        width: 100,
                        height: 100,
                        decoration: BoxDecoration(
                          gradient: const LinearGradient(
                            colors: [Color(0xFFDBEAFE), Color(0xFFBAE6FD)],
                            begin: Alignment.topLeft,
                            end: Alignment.bottomRight,
                          ),
                          borderRadius: BorderRadius.circular(22),
                        ),
                        child: const Stack(
                          alignment: Alignment.center,
                          children: [
                            Positioned(
                              top: 13,
                              right: 15,
                              child: Icon(
                                Icons.wb_sunny,
                                size: 32,
                                color: Color(0xFFFBBF24),
                              ),
                            ),
                            Positioned(
                              bottom: 15,
                              left: 12,
                              child: Icon(
                                Icons.cloud,
                                size: 62,
                                color: Color(0xFF38BDF8),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),

                  const Row(
                    mainAxisAlignment: MainAxisAlignment.spaceAround,
                    children: [
                      WeatherInfo(
                        icon: Icons.water_drop,
                        title: 'Độ ẩm',
                        value: '76%',
                      ),
                      WeatherInfo(
                        icon: Icons.air,
                        title: 'Gió',
                        value: '14 km/h',
                      ),
                      WeatherInfo(
                        icon: Icons.compress,
                        title: 'Áp suất',
                        value: '1009 hPa',
                      ),
                    ],
                  ),
                ],
              ),
            ),

            const SizedBox(height: 22),

            // DỰ BÁO 24 GIỜ
            const SectionTitle(
              title: 'Dự báo 24 giờ tới',
              subtitle: 'Theo giờ',
            ),
            const SizedBox(height: 12),

            SizedBox(
              height: 112,
              child: ListView(
                scrollDirection: Axis.horizontal,
                children: const [
                  HourlyItem(time: '13:00', temp: '29°', icon: Icons.wb_sunny),
                  HourlyItem(time: '14:00', temp: '30°', icon: Icons.wb_sunny),
                  HourlyItem(time: '15:00', temp: '31°', icon: Icons.wb_cloudy),
                  HourlyItem(time: '16:00', temp: '30°', icon: Icons.cloud),
                  HourlyItem(time: '17:00', temp: '28°', icon: Icons.cloud),
                  HourlyItem(
                    time: '18:00',
                    temp: '27°',
                    icon: Icons.nights_stay,
                  ),
                ],
              ),
            ),

            const SizedBox(height: 22),

            // BIỂU ĐỒ NHIỆT ĐỘ
            const SectionTitle(
              title: 'Biểu đồ nhiệt độ',
              subtitle: '24 giờ tới',
            ),
            const SizedBox(height: 12),

            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(18),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(18),
              ),
              child: Column(
                children: [
                  const Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'Nhiệt độ trong ngày',
                        style: TextStyle(
                          fontWeight: FontWeight.bold,
                          fontSize: 13,
                        ),
                      ),
                      Text(
                        '29°C',
                        style: TextStyle(
                          color: AppColors.primary,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 20),
                  SizedBox(
                    height: 110,
                    width: double.infinity,
                    child: CustomPaint(painter: TemperatureChartPainter()),
                  ),
                  const SizedBox(height: 10),
                  const Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text('06:00', style: TextStyle(fontSize: 10)),
                      Text('10:00', style: TextStyle(fontSize: 10)),
                      Text('14:00', style: TextStyle(fontSize: 10)),
                      Text('18:00', style: TextStyle(fontSize: 10)),
                      Text('22:00', style: TextStyle(fontSize: 10)),
                    ],
                  ),
                ],
              ),
            ),

            const SizedBox(height: 22),

            // CHỈ SỐ THỜI TIẾT
            const SectionTitle(
              title: 'Chất lượng không khí',
              subtitle: 'Thông tin môi trường',
            ),
            const SizedBox(height: 12),

            Row(
              children: [
                Expanded(
                  child: WeatherMetricCard(
                    icon: Icons.air,
                    title: 'Chất lượng không khí',
                    value: '45 AQI',
                    description: 'Tốt - Không khí sạch',
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Container(
                    padding: const EdgeInsets.all(14),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(16),
                    ),
                    child: const Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Icon(
                          Icons.wb_twilight,
                          color: AppColors.primary,
                          size: 20,
                        ),
                        SizedBox(height: 9),
                        Text('Mặt trời', style: TextStyle(fontSize: 11)),
                        SizedBox(height: 8),
                        Text(
                          '05:48',
                          style: TextStyle(
                            color: AppColors.darkBlue,
                            fontSize: 23,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        Text(
                          'Bình minh',
                          style: TextStyle(
                            color: AppColors.primary,
                            fontSize: 11,
                          ),
                        ),
                        SizedBox(height: 8),
                        Text(
                          '18:05  •  Hoàng hôn',
                          style: TextStyle(color: AppColors.grey, fontSize: 10),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

// ==========================================
// 4. CÁC WIDGET DÙNG CHUNG
// ==========================================

class SectionTitle extends StatelessWidget {
  final String title;
  final String subtitle;

  const SectionTitle({super.key, required this.title, required this.subtitle});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(
          title,
          style: const TextStyle(
            color: AppColors.darkBlue,
            fontSize: 15,
            fontWeight: FontWeight.bold,
          ),
        ),
        Text(
          subtitle,
          style: const TextStyle(color: AppColors.grey, fontSize: 11),
        ),
      ],
    );
  }
}

class WeatherInfo extends StatelessWidget {
  final IconData icon;
  final String title;
  final String value;

  const WeatherInfo({
    super.key,
    required this.icon,
    required this.title,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Icon(icon, color: AppColors.primary, size: 20),
        const SizedBox(height: 5),
        Text(title, style: const TextStyle(fontSize: 10)),
        const SizedBox(height: 3),
        Text(
          value,
          style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold),
        ),
      ],
    );
  }
}

class HourlyItem extends StatelessWidget {
  final String time;
  final String temp;
  final IconData icon;

  const HourlyItem({
    super.key,
    required this.time,
    required this.temp,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 65,
      margin: const EdgeInsets.only(right: 9),
      padding: const EdgeInsets.symmetric(vertical: 10),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(15),
      ),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.spaceAround,
        children: [
          Text(time, style: const TextStyle(fontSize: 11)),
          Icon(icon, color: Colors.orange, size: 23),
          Text(
            temp,
            style: const TextStyle(
              fontWeight: FontWeight.bold,
              color: AppColors.darkBlue,
            ),
          ),
        ],
      ),
    );
  }
}

class WeatherMetricCard extends StatelessWidget {
  final IconData icon;
  final String title;
  final String value;
  final String description;

  const WeatherMetricCard({
    super.key,
    required this.icon,
    required this.title,
    required this.value,
    required this.description,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, color: AppColors.primary, size: 20),
          const SizedBox(height: 9),
          Text(title, style: const TextStyle(fontSize: 11)),
          const SizedBox(height: 5),
          Text(
            value,
            style: const TextStyle(
              color: AppColors.darkBlue,
              fontSize: 23,
              fontWeight: FontWeight.bold,
            ),
          ),
          Text(
            description,
            style: const TextStyle(color: AppColors.primary, fontSize: 11),
          ),
        ],
      ),
    );
  }
}

// ==========================================
// 5. BIỂU ĐỒ NHIỆT ĐỘ
// ==========================================

class TemperatureChartPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final points = [
      Offset(0, size.height * 0.72),
      Offset(size.width * 0.2, size.height * 0.35),
      Offset(size.width * 0.4, size.height * 0.18),
      Offset(size.width * 0.6, size.height * 0.38),
      Offset(size.width * 0.8, size.height * 0.62),
      Offset(size.width, size.height * 0.78),
    ];

    final path = Path()..moveTo(points.first.dx, points.first.dy);

    for (int i = 0; i < points.length - 1; i++) {
      final p1 = points[i];
      final p2 = points[i + 1];

      path.cubicTo(
        p1.dx + (p2.dx - p1.dx) / 2,
        p1.dy,
        p1.dx + (p2.dx - p1.dx) / 2,
        p2.dy,
        p2.dx,
        p2.dy,
      );
    }

    final linePaint = Paint()
      ..color = AppColors.primary
      ..strokeWidth = 2.5
      ..style = PaintingStyle.stroke;

    canvas.drawPath(path, linePaint);

    final fillPath = Path.from(path)
      ..lineTo(size.width, size.height)
      ..lineTo(0, size.height)
      ..close();

    final fillPaint = Paint()
      ..shader = const LinearGradient(
        colors: [Color(0x6638BDF8), Color(0x0038BDF8)],
        begin: Alignment.topCenter,
        end: Alignment.bottomCenter,
      ).createShader(Rect.fromLTWH(0, 0, size.width, size.height));

    canvas.drawPath(fillPath, fillPaint);

    final pointPaint = Paint()
      ..color = AppColors.primary
      ..style = PaintingStyle.fill;

    for (final point in points) {
      canvas.drawCircle(point, 3, pointPaint);
    }
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

// ==========================================
// 6. MÀN HÌNH ĐỊA ĐIỂM
// ==========================================

class LocationScreen extends StatelessWidget {
  const LocationScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final cities = [
      ['TP. Hồ Chí Minh', '29°', 'Mây rải rác'],
      ['Hà Nội', '24°', 'Mưa phùn'],
      ['Đà Nẵng', '27°', 'Có gió nhẹ'],
      ['Cần Thơ', '30°', 'Nắng gián đoạn'],
      ['Hải Phòng', '23°', 'Nhiều mây'],
      ['Nha Trang', '28°', 'Trời trong xanh'],
      ['Đà Lạt', '18°', 'Se lạnh'],
    ];

    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Địa điểm',
              style: TextStyle(
                fontSize: 23,
                fontWeight: FontWeight.bold,
                color: AppColors.darkBlue,
              ),
            ),
            const SizedBox(height: 6),
            const Text(
              'Tìm kiếm và quản lý địa điểm thời tiết',
              style: TextStyle(color: AppColors.grey),
            ),
            const SizedBox(height: 18),
            TextField(
              decoration: InputDecoration(
                hintText: 'Tìm kiếm tỉnh, thành phố...',
                prefixIcon: const Icon(Icons.search),
                filled: true,
                fillColor: Colors.white,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(14),
                  borderSide: BorderSide.none,
                ),
              ),
            ),
            const SizedBox(height: 18),
            Expanded(
              child: ListView.builder(
                itemCount: cities.length,
                itemBuilder: (context, index) {
                  final city = cities[index];

                  return Card(
                    color: Colors.white,
                    elevation: 0,
                    margin: const EdgeInsets.only(bottom: 10),
                    child: ListTile(
                      leading: const CircleAvatar(
                        backgroundColor: AppColors.lightBlue,
                        child: Icon(Icons.wb_sunny, color: Colors.orange),
                      ),
                      title: Text(
                        city[0],
                        style: const TextStyle(fontWeight: FontWeight.bold),
                      ),
                      subtitle: Text(city[2]),
                      trailing: Text(
                        city[1],
                        style: const TextStyle(
                          fontSize: 21,
                          color: AppColors.darkBlue,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                  );
                },
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// ==========================================
// 7. MÀN HÌNH CÀI ĐẶT
// ==========================================

class SettingsScreen extends StatefulWidget {
  const SettingsScreen({super.key});

  @override
  State<SettingsScreen> createState() => _SettingsScreenState();
}

class _SettingsScreenState extends State<SettingsScreen> {
  bool notifications = true;
  bool severeAlerts = true;
  bool dailyForecast = true;

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          const Text(
            'Cài đặt',
            style: TextStyle(
              fontSize: 23,
              fontWeight: FontWeight.bold,
              color: AppColors.darkBlue,
            ),
          ),
          const SizedBox(height: 6),
          const Text(
            'Tùy chỉnh trải nghiệm VietWeather AI',
            style: TextStyle(color: AppColors.grey),
          ),
          const SizedBox(height: 20),
          const Text(
            'THÔNG BÁO',
            style: TextStyle(
              color: AppColors.primary,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 10),
          SwitchListTile(
            tileColor: Colors.white,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(14),
            ),
            title: const Text('Thông báo thời tiết'),
            subtitle: const Text('Nhận thông tin thời tiết mới nhất'),
            value: notifications,
            onChanged: (value) {
              setState(() => notifications = value);
            },
          ),
          const SizedBox(height: 8),
          SwitchListTile(
            tileColor: Colors.white,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(14),
            ),
            title: const Text('Cảnh báo thời tiết nguy hiểm'),
            value: severeAlerts,
            onChanged: (value) {
              setState(() => severeAlerts = value);
            },
          ),
          const SizedBox(height: 8),
          SwitchListTile(
            tileColor: Colors.white,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(14),
            ),
            title: const Text('Dự báo thời tiết mỗi ngày'),
            value: dailyForecast,
            onChanged: (value) {
              setState(() => dailyForecast = value);
            },
          ),
          const SizedBox(height: 22),
          const Text(
            'ĐƠN VỊ ĐO LƯỜNG',
            style: TextStyle(
              color: AppColors.primary,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 10),
          Card(
            color: Colors.white,
            elevation: 0,
            child: Column(
              children: const [
                ListTile(
                  leading: Icon(Icons.thermostat),
                  title: Text('Nhiệt độ'),
                  trailing: Text('°C'),
                ),
                Divider(height: 1),
                ListTile(
                  leading: Icon(Icons.air),
                  title: Text('Tốc độ gió'),
                  trailing: Text('km/h'),
                ),
                Divider(height: 1),
                ListTile(
                  leading: Icon(Icons.speed),
                  title: Text('Áp suất'),
                  trailing: Text('hPa'),
                ),
              ],
            ),
          ),
          const SizedBox(height: 22),
          const Text(
            'HỖ TRỢ VÀ PHẢN HỒI',
            style: TextStyle(
              color: AppColors.primary,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 10),
          Card(
            color: Colors.white,
            elevation: 0,
            child: const ListTile(
              leading: Icon(Icons.help_outline),
              title: Text('Trợ giúp và hỗ trợ'),
              trailing: Icon(Icons.chevron_right),
            ),
          ),
        ],
      ),
    );
  }
}
