using ColoritWPF.BLL;

namespace ColoritWPF.Common
{
    public class BaseVmWithBlls : BaseViewModel
    {
        private PrintHelper _printHelper;
        public PrintHelper PrintHelper
        {
            get { return _printHelper ?? (_printHelper = new PrintHelper()); }
        }
        
        private PaintNameBll _paintNameBll;
        public PaintNameBll PaintNameBll
        {
            get { return _paintNameBll ?? (_paintNameBll = new PaintNameBll()); }
        }

        private ProductsBll _productsBll;
        public ProductsBll ProductsBll
        {
            get { return _productsBll ?? (_productsBll = new ProductsBll()); }
        }

        private ClientsBll _clientsBllBll;
        public ClientsBll ClientsBll
        {
            get { return _clientsBllBll ?? (_clientsBllBll = new ClientsBll()); }
        }

        private DocumentsBll _documentsBll;
        public DocumentsBll DocumentsBll
        {
            get { return _documentsBll ?? (_documentsBll = new DocumentsBll()); }
        }

        private SettingsBll _settingsBll;
        public SettingsBll SettingsBll
        {
            get { return _settingsBll ?? (_settingsBll = new SettingsBll()); }
        }
    }
}
