using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace POMO
{
    public class TaskCreatedMessage : ValueChangedMessage<TaskModel>
    {
        public TaskCreatedMessage(TaskModel task) : base(task)
        {
        }
    }

    // Define the TaskAddedMessage class
    public class TaskAddedMessage
    {
        public TaskModel Task { get; }

        public TaskAddedMessage(TaskModel task)
        {
            Task = task;
        }
    }
}
