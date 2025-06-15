using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lớp tiện ích chứa các phương thức static hữu ích cho dự án.
/// Các phương thức trong class này có thể được sử dụng ở bất kỳ đâu trong project.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Đối tượng Random dùng để tạo số ngẫu nhiên.
    /// Được khởi tạo một lần và tái sử dụng để tránh việc tạo nhiều instance Random.
    /// </summary>
    public static System.Random r = new System.Random();

    /// <summary>
    /// Phương thức mở rộng để xáo trộn ngẫu nhiên các phần tử trong một danh sách.
    /// Sử dụng thuật toán Fisher-Yates shuffle để đảm bảo tính ngẫu nhiên và hiệu quả.
    /// 
    /// Cách sử dụng:
    /// 1. Với List<T>:
    ///    List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
    ///    numbers.Shuffle();
    /// 
    /// 2. Với bất kỳ collection nào implement IList<T>:
    ///    IList<string> names = new List<string> { "Alice", "Bob", "Charlie" };
    ///    names.Shuffle();
    /// 
    /// 3. Với array:
    ///    int[] array = { 1, 2, 3, 4, 5 };
    ///    array.Shuffle();
    /// 
    /// Lưu ý:
    /// - Phương thức này thay đổi trực tiếp danh sách gốc
    /// - Có thể sử dụng với bất kỳ kiểu dữ liệu T nào
    /// - Độ phức tạp: O(n) với n là số phần tử trong danh sách
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của các phần tử trong danh sách</typeparam>
    /// <param name="list">Danh sách cần xáo trộn</param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            // Chọn ngẫu nhiên một vị trí từ 0 đến n
            int k = r.Next(n + 1);
            // Hoán đổi phần tử tại vị trí k với phần tử tại vị trí n
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
