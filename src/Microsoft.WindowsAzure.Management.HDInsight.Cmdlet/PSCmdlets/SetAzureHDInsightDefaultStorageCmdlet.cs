﻿namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    /// <summary>
    /// Sets the Default Storage Container for the HDInsight cluster configuration.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, AzureHdInsightPowerShellHardCodes.AzureHDInsightDefaultStorage)]
    public class SetAzureHDInsightDefaultStorageCmdlet : AzureHDInsightCmdlet, ISetAzureHDInsightDefaultStorageBase
    {
        private ISetAzureHDInsightDefaultStorageCommand command;

        /// <summary>
        /// Initializes a new instance of the SetAzureHDInsightDefaultStorageCmdlet class.
        /// </summary>
        public SetAzureHDInsightDefaultStorageCmdlet()
        {
            this.command = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateSetDefaultStorage();
        }

        /// <summary>
        /// Gets or sets the Azure HDInsight Configuration for the Azure HDInsight cluster being constructed.
        /// </summary>
        [Parameter(Position = 0, Mandatory = true,
                   HelpMessage = "The HDInsight cluster configuration to use when creating the new cluster (created by New-AzureHDInsightConfig).",
                   ValueFromPipeline = true,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetDefaultStorageAccount)]
        public AzureHDInsightConfig Config
        {
            get { return this.command.Config; }
            set
            {
                if (value.IsNull())
                {
                    throw new ArgumentNullException("value",
                                                    "The value for the configuration can not be null.");
                }
                this.command.Config.ClusterSizeInNodes = value.ClusterSizeInNodes;
                this.command.Config.AdditionalStorageAccounts.AddRange(value.AdditionalStorageAccounts);
                this.command.Config.HiveMetastore = value.HiveMetastore ?? this.command.Config.HiveMetastore;
                this.command.Config.OozieMetastore = value.OozieMetastore ?? this.command.Config.OozieMetastore;
            }
        }

        /// <summary>
        /// Gets or sets the Storage Account name for the Default Storage Account.
        /// </summary>
        [Parameter(Position = 1, Mandatory = true,
                   HelpMessage = "The default storage account to use for the new cluster.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetDefaultStorageAccount)]
        public string StorageAccountName
        {
            get { return this.command.StorageAccountName; }
            set { this.command.StorageAccountName = value; }
        }

        /// <summary>
        /// Gets or sets the Storage Account key for the Default Storage Account.
        /// </summary>
        [Parameter(Position = 2, Mandatory = true,
                   HelpMessage = "The key to use for the default storage account.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetDefaultStorageAccount)]
        public string StorageAccountKey
        {
            get { return this.command.StorageAccountKey; }
            set { this.command.StorageAccountKey = value; }
        }

        /// <summary>
        /// Gets or sets the Storage Account container for the Default Storage Account.
        /// </summary>
        [Parameter(Position = 3, Mandatory = true,
                   HelpMessage = "The container in the storage account to use for default HDInsight storage.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetDefaultStorageAccount)]
        public string StorageContainerName
        {
            get { return this.command.StorageContainerName; }
            set { this.command.StorageContainerName = value; }
        }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
        }

        /// <inheritdoc />
        protected override void EndProcessing()
        {
            this.command.EndProcessing();
            foreach (var output in this.command.Output)
            {
                this.WriteObject(output);
            }
        }
    }
}
