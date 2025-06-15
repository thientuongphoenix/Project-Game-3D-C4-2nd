using UnityEngine;

//Random Selector
public class RSelector : Node
{
    bool shuffle = false; //Chạy lần đầu thì khỏi cần shuffle
    //Shuffle rồi thì không cần shuffle lại nữa

    public RSelector(string n)
    {
        this.name = n;
    }

    public override Status Process()
    {
        // Đây là hàm xử lý chính của Random Selector node trong Behavior Tree
        // Random Selector sẽ chọn ngẫu nhiên 1 node con để thực thi

        // Nếu chưa shuffle thì thực hiện xáo trộn thứ tự các node con
        if(!shuffle)
        {
            children.Shuffle(); // Xáo trộn mảng các node con
            shuffle = true; // Đánh dấu đã shuffle
        }

        // Thực thi node con hiện tại và lấy trạng thái
        Status childstatus = children[currentChild].Process();

        // Nếu node con đang chạy thì return RUNNING
        if(childstatus == Status.RUNNING) return Status.RUNNING;

        // Nếu node con thành công
        if (childstatus == Status.SUCCESS)
        {
            currentChild = 0; // Reset index node con về 0
            shuffle = false; // Reset trạng thái shuffle
            return Status.SUCCESS; // Return SUCCESS vì đã có 1 node con thành công
        }

        // Nếu node con thất bại thì chuyển sang node tiếp theo
        currentChild++;
        if(currentChild >= children.Count)
        {
            currentChild = 0; // Reset index về 0
            shuffle = false; // Reset trạng thái shuffle
            return Status.FAILURE; // Return FAILURE vì đã thử hết các node con mà không có cái nào SUCCESS
        }

        // Tiếp tục chạy node con tiếp theo
        return Status.RUNNING;
    }
}
