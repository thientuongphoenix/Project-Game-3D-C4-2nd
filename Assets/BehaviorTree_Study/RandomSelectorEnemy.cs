using System.Collections.Generic;
using UnityEngine;

//Random Selector
public class RandomSelectorEnemy : Node
{
    private List<Node> childrenEnemy;
    private List<float> weightsEnemy;
    private System.Random random = new System.Random();

    // Thêm biến lưu node đang theo đuổi
    private Node currentNode = null;

    public RandomSelectorEnemy(List<Node> childrenEnemy, List<float> weightsEnemy)
    {
        this.childrenEnemy = childrenEnemy;
        this.weightsEnemy = weightsEnemy;
    }

    public override Status Process()
    {
        // Nếu đang chase Player/Tower thì giữ nguyên node đó
        if (currentNode != null)
        {
            // Nếu node hiện tại là GoToPlayer hoặc GoToNearestTower thì giữ nguyên
            if (currentNode == childrenEnemy[1] || currentNode == childrenEnemy[2])
            {
                var status = currentNode.Process();
                if (status == Status.RUNNING)
                    return Status.RUNNING;
                currentNode = null;
                return status;
            }
            // Nếu node hiện tại là GoToNextPoint
            if (currentNode == childrenEnemy[0])
            {
                // Kiểm tra nếu có Player hoặc Tower thì random lại
                var playerStatus = childrenEnemy[1].Process();
                var towerStatus = childrenEnemy[2].Process();
                if (playerStatus == Status.RUNNING || towerStatus == Status.RUNNING)
                {
                    currentNode = null; // Bỏ node GoToNextPoint để random lại
                }
                else
                {
                    var status = currentNode.Process();
                    if (status == Status.RUNNING)
                        return Status.RUNNING;
                    currentNode = null;
                    return status;
                }
            }
        }

        // Nếu chưa có mục tiêu, random như cũ
        float total = 0;
        foreach (var w in weightsEnemy) total += w;
        float r = (float)(random.NextDouble() * total);
        float sum = 0;
        for (int i = 0; i < childrenEnemy.Count; i++)
        {
            sum += weightsEnemy[i];
            if (r <= sum)
            {
                var status = childrenEnemy[i].Process();
                if (status == Status.RUNNING)
                {
                    currentNode = childrenEnemy[i];
                    return Status.RUNNING;
                }
                if (status == Status.FAILURE)
                {
                    // Nếu node này không khả dụng, thử random lại node khác
                    float newR = (float)(random.NextDouble() * total);
                    sum = 0;
                    for (int j = 0; j < childrenEnemy.Count; j++)
                    {
                        if (j == i) continue;
                        sum += weightsEnemy[j];
                        if (newR <= sum)
                        {
                            var status2 = childrenEnemy[j].Process();
                            if (status2 == Status.RUNNING)
                            {
                                currentNode = childrenEnemy[j];
                                return Status.RUNNING;
                            }
                            return status2;
                        }
                    }
                }
                return status;
            }
        }
        return Status.FAILURE;
    }
}
