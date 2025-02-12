﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace POMO
{
    public class TaskSelectedMessage : ValueChangedMessage<TaskModel>
    {
        public TaskSelectedMessage(TaskModel task) : base(task)
        {
        }
    }

    public class DueTaskSelectedMessage : ValueChangedMessage<(int Id, string Title, int CompletedSessions, int NumSessions)>
    {
        public DueTaskSelectedMessage((int Id, string Title, int CompletedSessions, int NumSessions) value) : base(value)
        {
        }
    }
}
