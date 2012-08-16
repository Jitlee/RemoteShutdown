#region Copyright 2011 - 2012 Information Security Software (China) Co., Ltd.
//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright 2011 - 2012 Information Security Software (China) Co., Ltd.
// All rights reserved
// 版权所有 2011 - 2012 信安软件（中国）有限公司
// 保留所有权利
//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Note:
// This file contains confidential information and trade secret of Information Security
// Software (China) Co., Ltd., a wholly-owned subsidiary of China Information Technology, Inc.
// You can not give the information of this file to other parties without the written
// authorization of Information Security Software (China) Co., Ltd.
// 注意:
// 本文件包含信安软件（中国）有限公司的机密信息和商业秘密。
// 没有信安软件（中国）有限公司的书面授权，严禁向第三方泄漏本文件的相关信息。
// 信安软件（中国）有限公司为中国信息技术有限公司的全资子公司。
//////////////////////////////////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Windows.Input;

namespace RemoteShutdown.Core
{
    public class DelegateCommandBase : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommandBase(Action<object> execute)
        {
            _execute = execute;
        }

        public DelegateCommandBase(Action<object> execute,Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }

    public class DelegateCommand<T> : DelegateCommandBase
    {
        public DelegateCommand(Action<T> execute) : 
            base(new Action<object>(o => {
                if (null != execute)
                {
                    execute((T)o);
                }
            })) { }

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecuteMethod) :
            base(new Action<object>(o =>
            {
                if (null != execute)
                {
                    execute((T)o);
                }
            }), new Predicate<object>(o => {
                if (null != canExecuteMethod)
                {
                    return canExecuteMethod((T)o);
                }
                return true;
            })) { }

        public DelegateCommand(Action<T> execute, Func<bool> canExecuteMethod) :
            base(new Action<object>(o =>
            {
                if (null != execute)
                {
                    execute((T)o);
                }
            }), new Predicate<object>(o =>
            {
                if (null != canExecuteMethod)
                {
                    return canExecuteMethod();
                }
                return true;
            })) { }
    }

    public class DelegateCommand : DelegateCommandBase
    {
        public DelegateCommand(Action execute) :
            base(new Action<object>(o =>
            {
                if (null != execute)
                {
                    execute();
                }
            })) { }

        public DelegateCommand(Action execute, Func<bool> canExecuteMethod) :
            base(new Action<object>(o =>
            {
                if (null != execute)
                {
                    execute();
                }
            }), new Predicate<object>(o =>
            {
                if (null != canExecuteMethod)
                {
                    return canExecuteMethod();
                }
                return true;
            })) { }
    }
}
