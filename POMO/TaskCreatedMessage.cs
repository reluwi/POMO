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
}
