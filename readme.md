# Trò chơi tháp Hà Nội 
----
# Thông tin trò chơi:

Chương  trình cho phép Server đóng vai trò BTC và trọng tài của trò chơi “Tháp Hà Nội”, trò chơi cần 3 người chơi (3 người chơi khác nhau). 
Trò chơi sẽ thực hiện như sau:
1. Với mỗi Client, Client sẽ đăng ký với Server nickname mà mình sử dụng ngay sau khi kết nối  thành công với Server. Lưu ý: các nickname không được đặt trùng nhau và được tạo thành từ các ký tự ‘a’…’z’, ‘A’…’Z’, ‘0’..’9’ và dài không quá 10 ký tự, nếu có client đặt trùng nickname với client khác, thì server yêu cầu client đăng ký lại nickname.
2. Khi Server nhận được thông tin đăng ký của 3 Client.  Server sẽ thông báo trò chơi bắt đầu.
3. Với mỗi lượt chơi:
a. Server sẽ random số lượng đĩa và vị trí của các đĩa trên các cột (số đĩa từ 3 - 6 đĩa; số cột: 3 cột (A B C)) rồi xuất ra màn hình cho các thí sinh (vị trí đĩa và số lượng đĩa trên các cột của các thí sinh giống nhau)
Ví dụ: cột A chứa đĩa 3 và 2; cột B chứ đĩa 4 và 1; cột C chứ đĩa 6 và 5 thì xuất như sau:
A: 3 2
B: 4 1
C: 6 5
b. Các thí sinh lần lượt gởi cho server vị trí mới của đĩa trên cột (ví dụ: đĩa 1 cho qua cột A: 1-A; đĩa 6 cho qua cột C: 6-C …)
c. Khi server nhận được 1 lần di chuyển của từ 1 thí sinh nào đó. Kiểm tra đĩa vừa dịch chuyển có kích thứơc nhỏ hơn đĩa trước không?
• Nếu không thì yêu cầu thí sinh đó di chuyển lại
• Nếu có thì:
i. Nếu đó là vị trí kết thúc trò chơi (tại cột C các đĩa được sắp xếp theo thứ tự giảm dần về kích thước của các đĩa: 6 5 4 3 2 1) thì server thông báo cho các thí sinh trò chơi kết thúc và xuất kết quả dịch chuyển ra màn hình.
ii. Nếu đó là vị trí không phải kết thúc trò chơi (tại cột C các đĩa vẫn chưa được sắp xếp  theo thứ tự giảm dần về kích thước của đĩa) thì server sẽ thông báo kết quả dịch chuyển của thí sinh.
d. Server tính điểm cho các thí sinh sau mõi lần dịch chuyển. Sau mõi lần dịch chuyển thì thí sinh được tăng lên 1 điểm (số lần dịch chuyển càng ít = số điểm ít thì hạng của thí sinh đó sẽ cao).
4. Trò chơi kết thúc khi các thí sinh tìm được lời giải cho trò chơi hoặc thí sinh yêu cầu kết thúc trò chơi (khi thí sinh đó vẫn chưa tìm được lời giải). Server sẽ gởi kết quả thắng cuộc cho các thí sinh và số điểm của người chơi.
5. Kết thúc chương trình.
