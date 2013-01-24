using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProcessRunner.Views
{
    public interface IView
    {
        void NavigatingTo();
        void NavigatedTo();
        void NavigatingFrom();
        void NavigatedFrom();
    }
}
