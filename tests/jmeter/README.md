# Kiểm thử Tải và An Ninh Mạng bằng JMeter (Load & Security Testing)

Thư mục này chứa các kịch bản kiểm thử (Test Plan) sử dụng **Apache JMeter** cho dự án Ebay Clone.

## 1. Yêu cầu hệ thống
- Tải và cài đặt **[Apache JMeter](https://jmeter.apache.org/download_jmeter.cgi)** (Yêu cầu Java 8+).
- Đảm bảo Backend (API) của dự án đang chạy (Mặc định ở `http://localhost:5000`).

## 2. Kịch bản kiểm thử (`LoadAndSecurityTest.jmx`)
File `LoadAndSecurityTest.jmx` bao gồm 2 phần kiểm thử chính:

### a. Load Test (Kiểm thử tải) - `GET All Products`
- **Mục tiêu**: Kiểm tra khả năng chịu tải của API lấy danh sách sản phẩm.
- **Cấu hình**: 50 Users (Threads) x 10 Vòng lặp (Loops) = 500 requests liên tục.
- **Đánh giá**: Sử dụng *Summary Report* để xem thời gian phản hồi trung bình (Average Response Time), tỷ lệ lỗi (Error %) và số request có thể xử lý mỗi phút/giây (Throughput).

### b. Security Test (Kiểm thử bảo mật) - `SQL Injection Attempt`
- **Mục tiêu**: Kiểm tra API Đăng nhập có bị lỗ hổng SQL Injection không.
- **Payload**: `{ "email": "admin' OR '1'='1", "password": "password123" }`
- **Cấu hình**: 5 Threads gọi đồng thời.
- **Đánh giá**: Cài đặt **Response Assertion** để đảm bảo máy chủ phải trả về mã lỗi `401 Unauthorized` hoặc `404 Not Found` thay vì `200 OK` (chặn thành công truy cập trái phép).

## 3. Hướng dẫn chạy Test
1. Mở ứng dụng **JMeter** (chạy file `jmeter.bat` trên Windows hoặc `./jmeter.sh` trên Linux/Mac).
2. Tới **File** -> **Open** và chọn file `LoadAndSecurityTest.jmx` trong thư mục này.
3. Chỉnh sửa biến `BASE_URL` và `PORT` trong phần *User Defined Variables* ở gốc của Test Plan nếu server của bạn chạy ở port khác cổng 5000.
4. Bấm nút màu xanh lá (Start) trên thanh công cụ để chạy toàn bộ kịch bản.
5. Mở các màn hình điều khiển như **View Results Tree** và **Summary Report** để xem kết quả trực tiếp phản hồi của các API.
