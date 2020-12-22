using System.ServiceProcess;
using Libraris;

namespace Lab4
{
    public partial class DataManager : ServiceBase
    {
        readonly DatabaseConf appInsights;
        readonly DataOptions dataOptions;
        public DataManager(DataOptions dataOptions, DatabaseConf appInsights)
        {
            InitializeComponent();
            this.dataOptions = dataOptions;
            this.appInsights = appInsights;
        }
        protected override void OnStart(string[] args)
        {
            DatabaseConf reader = new DatabaseConf(dataOptions.ConnectionString);
            FileTransfer fileTransfer = new FileTransfer(dataOptions.OutputFolder, dataOptions.SourcePath);
            string ParamsFileName = "Params";
            reader.GetParams(dataOptions.OutputFolder, appInsights, ParamsFileName);
            fileTransfer.SendFileToFtp($"{ParamsFileName}.xml");
            fileTransfer.SendFileToFtp($"{ParamsFileName}.xsd");
            appInsights.InsertInsight("Files uploaded");
        }
        protected override void OnStop()
        {
            appInsights.InsertInsight("Stop");
            appInsights.WriteInsightsToXml(dataOptions.OutputFolder);
        }
    }
}