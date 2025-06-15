using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PSelector (Priority Selector) là một node trong Behavior Tree thực hiện việc chọn và thực thi các node con theo thứ tự ưu tiên.
/// Node này sẽ thực thi các node con theo thứ tự được sắp xếp bởi sortOrder, và trả về SUCCESS ngay khi một node con thành công.
/// 
/// Đặc điểm quan trọng của PSelector:
/// 1. Tự động sắp xếp các node con theo độ ưu tiên (sortOrder) trước khi thực thi
/// 2. Học hỏi từ kinh nghiệm: Node thành công sẽ được ưu tiên cao hơn (sortOrder = 1)
/// 3. Node thất bại sẽ bị giảm độ ưu tiên (sortOrder = 10) nhưng vẫn được giữ lại trong cây
/// 4. Vẫn tuân thủ nguyên tắc "từ trái sang phải" của Behavior Tree, nhưng thứ tự các node có thể thay đổi
///    dựa trên độ ưu tiên trước mỗi lần thực thi
/// </summary>
public class PSelector : Node
{
    Node[] nodeArray;
    bool ordered = false; //Chạy lần đầu thì khỏi cần sắp xếp lại

    /// <summary>
    /// Khởi tạo một PSelector node với tên được chỉ định
    /// </summary>
    /// <param name="n">Tên của node</param>
    public PSelector(string n)
    {
        this.name = n;
    }

    /// <summary>
    /// Sắp xếp các node con theo thứ tự ưu tiên (sortOrder) sử dụng thuật toán QuickSort.
    /// Việc sắp xếp này được thực hiện trước mỗi lần Process() để đảm bảo các node con
    /// được thực thi theo đúng thứ tự ưu tiên mới nhất.
    /// </summary>
    public void OrderNode()
    {
        nodeArray = children.ToArray();
        Sort(nodeArray, 0, nodeArray.Length - 1);
        children = new List<Node>(nodeArray);
    }

    /// <summary>
    /// Xử lý logic của PSelector node:
    /// 1. Sắp xếp lại các node con theo thứ tự ưu tiên
    /// 2. Thực thi node con hiện tại
    /// 3. Nếu node con thành công:
    ///    - Đặt sortOrder = 1 để tăng độ ưu tiên cho lần sau
    ///    - Reset currentChild và trả về SUCCESS
    /// 4. Nếu node con thất bại:
    ///    - Đặt sortOrder = 10 để giảm độ ưu tiên cho lần sau
    ///    - Chuyển sang node con tiếp theo
    /// 5. Nếu đã thử hết các node con mà không có node nào thành công:
    ///    - Reset currentChild và trả về FAILURE
    /// 6. Nếu node con đang thực thi hoặc chưa thử hết:
    ///    - Trả về RUNNING để tiếp tục ở lần update tiếp theo
    /// </summary>
    /// <returns>Status của node (SUCCESS/FAILURE/RUNNING)</returns>
    public override Status Process()
    {
        // Sắp xếp lại các node con theo thứ tự ưu tiên
        if(!ordered)
        {
            OrderNode();
            ordered = true;
        }

        // Thực thi node con hiện tại
        Status childstatus = children[currentChild].Process();
        if(childstatus == Status.RUNNING) return Status.RUNNING;

        // Nếu node con thành công, tăng độ ưu tiên và trả về SUCCESS
        if (childstatus == Status.SUCCESS)
        {
            //children[currentChild].sortOrder = 1; // Tăng độ ưu tiên cho lần sau
            currentChild = 0; // Reset cho lần sau
            ordered = false;
            return Status.SUCCESS;
        }
        //else children[currentChild].sortOrder = 10; // Giảm độ ưu tiên cho lần sau

        // Nếu node con thất bại, chuyển sang node con tiếp theo
        currentChild++;
        if(currentChild >= children.Count)
        {
            currentChild = 0;
            ordered = false;
            return Status.FAILURE; // Không có node nào thành công thì node Fail
        }

        return Status.RUNNING;
    }

    /// <summary>
    /// Hàm phân hoạch (partition) cho thuật toán QuickSort.
    /// Sắp xếp các node dựa trên sortOrder, với sortOrder nhỏ hơn = độ ưu tiên cao hơn.
    /// </summary>
    /// <param name="array">Mảng node cần sắp xếp</param>
    /// <param name="low">Chỉ số bắt đầu</param>
    /// <param name="high">Chỉ số kết thúc</param>
    /// <returns>Vị trí của pivot sau khi phân hoạch</returns>
    int Partition(Node[] array, int low, int high)
    {
        Node pivot = array[high];

        int lowIndex = (low - 1);

        // Sắp xếp các phần tử nhỏ hơn pivot về bên trái
        for(int j = low; j < high; j++)
        {
            if(array[j].sortOrder <= pivot.sortOrder)
            {
                lowIndex++;

                Node temp = array[lowIndex];
                array[lowIndex] = array[j];
                array[j] = temp;
            }
        }

        // Đặt pivot vào vị trí đúng
        Node temp1 = array[lowIndex + 1];
        array[lowIndex + 1] = array[high];
        array[high] = temp1;

        return lowIndex + 1;
    }
    
    /// <summary>
    /// Hàm QuickSort để sắp xếp các node theo sortOrder.
    /// Node có sortOrder nhỏ hơn sẽ được đặt bên trái (độ ưu tiên cao hơn).
    /// </summary>
    /// <param name="array">Mảng node cần sắp xếp</param>
    /// <param name="low">Chỉ số bắt đầu</param>
    /// <param name="high">Chỉ số kết thúc</param>
    void Sort(Node[] array, int low, int high)
    {
        if(low < high)
        {
            int partitionIndex = Partition(array, low, high);

            // Đệ quy sắp xếp các phần tử bên trái và phải của pivot
            Sort(array, low, partitionIndex - 1);
            Sort(array, partitionIndex + 1, high);
        }
    }
}
