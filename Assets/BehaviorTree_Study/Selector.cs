using UnityEngine;

public class Selector : Node
{
    public Selector(string n)
    {
        this.name = n;
    }

    public override Status Process()
    {
        //---------------------Penny de Bel code----------------------
        Status childstatus = children[currentChild].Process();
        if(childstatus == Status.RUNNING) return Status.RUNNING;

        if (childstatus == Status.SUCCESS)
        {
            currentChild = 0; // Reset cho lần sau
            return Status.SUCCESS;
        }

        currentChild++;
        if(currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.FAILURE; //Không có cái nào SUCCESS hết thì node Fail
        }

        return Status.RUNNING;
        //------------------------------------------------------------
        //// Gọi Process() của node con hiện tại
        //Status childStatus = children[currentChild].Process();

        //// Nếu node con thành công, Selector thành công ngay lập tức
        //if (childStatus == Status.SUCCESS)
        //{
        //    currentChild = 0; // Reset cho lần sau
        //    return Status.SUCCESS;
        //}

        //// Nếu node con đang chạy, Selector cũng báo RUNNING
        //if (childStatus == Status.RUNNING)
        //{
        //    return Status.RUNNING;
        //}

        //// Nếu node con thất bại, kiểm tra node con tiếp theo
        //// Đã có SUCCESS và RUNNING rồi, thì code chạy tới khúc này chỉ có FAILURE
        //currentChild++;
        //if (currentChild >= children.Count)
        //{
        //    // Nếu đã duyệt hết tất cả mà không có thành công → Selector thất bại
        //    currentChild = 0;
        //    return Status.FAILURE;
        //}

        //// Nếu chưa duyệt hết, tiếp tục kiểm tra node tiếp theo
        //return Status.RUNNING;
    }
}
