using System.ComponentModel;
using InWit.Core.Utils;

namespace InWit.Core.Tests.Utils
{
    public class TestContent : INotifyPropertyChanged
    {
        #region Events

#pragma warning disable 67
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
#pragma warning restore 67

        #endregion

        #region Fields

        private double m_data;

        private double m_additional;

        #endregion

        #region Constructors

        public TestContent(double data)
        {
            Data = data;
        }

        public TestContent()
            : this(0)
        {
            
        }

        #endregion

        #region Properties

        public double Data
        {
            get { return m_data; }
            set
            {
                m_data = value;
                this.FirePropertyChanged();
            }
        }

        public double Additional
        {
            get { return m_additional; }
            set
            {
                m_additional = value;
                this.FirePropertyChanged(x => Additional);
            }
        }

        #endregion
    } 
}








