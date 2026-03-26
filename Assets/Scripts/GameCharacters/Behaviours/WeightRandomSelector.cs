using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom")]
    [TaskDescription("权重随机Selector")]
    [TaskIcon("{SkinColor}SelectorIcon.png")]
    public class WeightRandomSelector : Composite
    {
        [Tooltip("每个子任务的概率（总和应为1）")]
        public List<float> probabilities = new List<float>();

        [Tooltip("是否在每次执行时重新选择")]
        public bool reselect = false;

        // 当前选择的子任务索引
        private int currentChildIndex = -1;
        // 当前正在执行的任务
        private TaskStatus executionStatus = TaskStatus.Inactive;

        // 当任务开始执行时调用
        public override void OnStart()
        {
            if(probabilities.Count > children.Count)
            {
                probabilities.GetRange(0, children.Count);
            }
            // 如果之前没有选择或者需要重新选择，则进行随机选择
            if (currentChildIndex == -1 || reselect)
            {
                SelectRandomChild();
            }

            executionStatus = TaskStatus.Running;
        }

        // 根据概率随机选择一个子任务
        private void SelectRandomChild()
        {
            float randomValue = Random.value;
            float cumulative = 0f;

            for (int i = 0; i < probabilities.Count; i++)
            {
                cumulative += probabilities[i];
                if (randomValue <= cumulative || i == probabilities.Count - 1)
                {
                    currentChildIndex = i;
                    // Debug.Log($"概率选择器: 随机值={randomValue:F2}, 选择任务{i} (概率={probabilities[i]:P0})");
                    return;
                }
            }

            // 兜底
            currentChildIndex = probabilities.Count - 1;
        }

        // Behavior Designer 调用此方法获取当前应执行的子任务索引
        public override int CurrentChildIndex()
        {
            return currentChildIndex;
        }

        // 检查是否可以执行
        public override bool CanExecute()
        {
            // 确保子任务数量与概率数组匹配
            if (children.Count != probabilities.Count)
            {
                Debug.LogError($"概率选择器: 子任务数量({children.Count})与概率数量({probabilities.Count})不匹配！");
                return false;
            }

            // 检查概率总和是否接近1（允许一定误差）
            float sum = 0f;
            foreach (float prob in probabilities)
            {
                sum += prob;
            }

            if (Mathf.Abs(sum - 1f) > 0.01f)
            {
                Debug.LogWarning($"概率选择器: 概率总和={sum:F2}, 应为1.0");
            }

            return currentChildIndex >= 0 && currentChildIndex < children.Count && (executionStatus != TaskStatus.Success);
        }

        // 当一个子任务完成时调用
        public override void OnChildExecuted(TaskStatus childStatus)
        {
            executionStatus = childStatus;

            // 如果选择的任务失败且需要重选
            if (childStatus == TaskStatus.Failure && reselect)
            {
                SelectRandomChild();
                executionStatus = TaskStatus.Running;
            }
        }

        // 覆盖默认的状态
        public override TaskStatus OverrideStatus(TaskStatus status)
        {
            return executionStatus;
        }

        // 检查是否还有子任务需要执行
        public override bool CanRunParallelChildren()
        {
            return false;
        }
        public override void OnConditionalAbort(int childIndex)
        {
            // Start from the beginning on an abort
            executionStatus = TaskStatus.Inactive;
            SelectRandomChild();
        }

        // 当任务结束时重置状态
        public override void OnEnd()
        {
            // 如果不保留选择，重置索引
            if (reselect)
            {
                currentChildIndex = -1;
            }
            executionStatus = TaskStatus.Inactive;
        }

        // 重置任务状态
        public override void OnReset()
        {
            currentChildIndex = -1;
            executionStatus = TaskStatus.Inactive;
        }
    }
}