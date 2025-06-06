﻿//
// Copyright (c) Seal Report (sealreport@gmail.com), http://www.sealreport.org.
// Licensed under the Seal Report Dual-License version 1.0; you may not use this file except in compliance with the License described at https://github.com/ariacom/Seal-Report.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.ComponentModel;
using Seal.Helpers;
#if WINDOWS
using DynamicTypeDescriptor;
using System.Drawing.Design;
using Seal.Forms;
using System.ComponentModel.Design;
#endif

namespace Seal.Model
{
    /// <summary>
    /// A ReportOutput defines the execution of a report on an OutputDevice (Folder or Email)
    /// </summary>
    public class ReportOutput : ReportComponent
    {
#if WINDOWS
        #region Editor

        protected override void UpdateEditorAttributes()
        {
            if (_dctd != null)
            {
                //Disable all properties
                foreach (var property in Properties) property.SetIsBrowsable(false);
                //Then enable
                GetProperty("DeviceName").SetIsBrowsable(true);
                GetProperty("UseCustomRestrictions").SetIsBrowsable(true);
                GetProperty("Restrictions").SetIsBrowsable(true);
                GetProperty("ViewGUID").SetIsBrowsable(true);
                GetProperty("CancelIfNoRecords").SetIsBrowsable(true);
                GetProperty("PreScript").SetIsBrowsable(true);
                GetProperty("PostScript").SetIsBrowsable(true);
                GetProperty("ViewParameters").SetIsBrowsable(true);

                GetProperty("FolderPath").SetIsBrowsable(Device is OutputFolderDevice || Device is OutputFileServerDevice);
                if (Device is OutputFileServerDevice) GetProperty("FolderPath").SetDescription("Path of the folder used to generate the report result.");
                GetProperty("FileName").SetIsBrowsable(Device is OutputFolderDevice || Device is OutputFileServerDevice);

                GetProperty("EmailSubject").SetIsBrowsable(Device is OutputEmailDevice);
                GetProperty("EmailBody").SetIsBrowsable(Device is OutputEmailDevice);
                GetProperty("EmailHtmlBody").SetIsBrowsable(Device is OutputEmailDevice);
                GetProperty("EmailTo").SetIsBrowsable(Device is OutputEmailDevice);
                GetProperty("EmailCC").SetIsBrowsable(Device is OutputEmailDevice);
                GetProperty("EmailBCC").SetIsBrowsable(Device is OutputEmailDevice);
                GetProperty("EmailMessagesInBody").SetIsBrowsable(Device is OutputEmailDevice);
                GetProperty("ZipResult").SetIsBrowsable(true);
                GetProperty("ZipPassword").SetIsBrowsable(true);
                GetProperty("EmailSkipAttachments").SetIsBrowsable(Device is OutputEmailDevice);
                GetProperty("EmailFrom").SetIsBrowsable(Device is OutputEmailDevice && ((OutputEmailDevice)Device).ChangeSender);
                GetProperty("EmailReplyTo").SetIsBrowsable(Device is OutputEmailDevice && ((OutputEmailDevice)Device).ChangeSender);

                GetProperty("UserName").SetIsBrowsable(true);
                GetProperty("UserGroups").SetIsBrowsable(true);
                GetProperty("UserCulture").SetIsBrowsable(true);
                GetProperty("PublicExec").SetIsBrowsable(true);
                GetProperty("PublicEdit").SetIsBrowsable(true);

                //Helpers
                //GetProperty("Information").SetIsBrowsable(true);
                //GetProperty("Error").SetIsBrowsable(true);

                //Readonly
                GetProperty("Restrictions").SetIsReadOnly(!UseCustomRestrictions);
                GetProperty("EmailBody").SetIsReadOnly(EmailHtmlBody || EmailMessagesInBody);
                GetProperty("EmailHtmlBody").SetIsReadOnly(EmailMessagesInBody);
                GetProperty("EmailMessagesInBody").SetIsReadOnly(EmailHtmlBody);

                GetProperty("EmailSkipAttachments").SetIsReadOnly(EmailHtmlBody);
                GetProperty("ZipResult").SetIsReadOnly(Device is OutputEmailDevice && (EmailHtmlBody || EmailSkipAttachments));
                GetProperty("ZipPassword").SetIsReadOnly(!ZipResult || (Device is OutputEmailDevice && (EmailHtmlBody || EmailSkipAttachments)));

                TypeDescriptor.Refresh(this);
            }
        }
        #endregion

#endif
        /// <summary>
        /// Basic creation
        /// </summary>
        public static ReportOutput Create()
        {
            return new ReportOutput() { GUID = Guid.NewGuid().ToString() };
        }

        /// <summary>
        /// Init all references
        /// </summary>
        public void InitReferences()
        {
            var initialParameters = ViewParameters.Where(i => i.CustomValue).ToList();
            ViewParameters.Clear();
            if (Report != null && View != null)
            {
                foreach (var configParameter in View.Template.Parameters)
                {
                    OutputParameter parameter = initialParameters.FirstOrDefault(i => i.Name == configParameter.Name);
                    if (parameter == null) parameter = new OutputParameter() { Name = configParameter.Name, Value = configParameter.Value };
                    else parameter.CustomValue = true;
                    ViewParameters.Add(parameter);
                    parameter.Enums = configParameter.Enums;
                    parameter.Description = configParameter.Description;
                    parameter.Type = configParameter.Type;
                    parameter.UseOnlyEnumValues = configParameter.UseOnlyEnumValues;
                    parameter.DisplayName = configParameter.DisplayName;
                    parameter.ConfigValue = configParameter.Value;
                    parameter.EditorLanguage = configParameter.EditorLanguage;
                    parameter.TextSamples = configParameter.TextSamples;
                }
            }
        }

        private string _outputDeviceGUID;
        /// <summary>
        /// Identifier of the output device used by the output
        /// </summary>
        public string OutputDeviceGUID
        {
            get
            {
                if (string.IsNullOrEmpty(_outputDeviceGUID)) _outputDeviceGUID = OutputFolderDevice.DefaultGUID;
                return
                    _outputDeviceGUID;
            }
            set { _outputDeviceGUID = value; }
        }

        /// <summary>
        /// The device used by this output
        /// </summary>
#if WINDOWS
        [Category("Definition"), DisplayName("Output device"), Description("The device used by this output."), Id(1, 1)]
#endif
        [XmlIgnore]
        public string DeviceName
        {
            get
            {
                return Device.FullName;
            }
        }

        /// <summary>
        /// If the models of the report do not have any record, the output generation is cancelled
        /// </summary>
#if WINDOWS
        [DefaultValue(false)]
        [Category("Definition"), DisplayName("Cancel generation if no records"), Description("If the models of the report do not have any record, the output generation is cancelled."), Id(3, 1)]
#endif
        public bool CancelIfNoRecords { get; set; } = false;

        /// <summary>
        /// Optional Razor script executed before the generation. If the script returns 0, the generation is aborted.
        /// </summary>
#if WINDOWS
        [Category("Definition"), DisplayName("Pre-generation script"), Description("Optional Razor script executed before the generation. If the script returns 0, the generation is aborted."), Id(4, 1)]
        [Editor(typeof(TemplateTextEditor), typeof(UITypeEditor))]
#endif
        public string PreScript { get; set; }

        /// <summary>
        /// Optional Razor script executed after the generation
        /// </summary>
#if WINDOWS
        [Category("Definition"), DisplayName("Post-generation script"), Description("Optional Razor script executed after the generation."), Id(5, 1)]
        [Editor(typeof(TemplateTextEditor), typeof(UITypeEditor))]
#endif
        public string PostScript { get; set; }

        /// <summary>
        /// Custom parameters used for the Root View when the output is executed
        /// </summary>
#if WINDOWS
        [Category("Definition"), DisplayName("Custom view parameters"), Description("Custom parameters used for the Root View when the output is executed."), Id(6, 1)]
        [Editor(typeof(EntityCollectionEditor), typeof(UITypeEditor))]
#endif
        public List<OutputParameter> ViewParameters { get; set; } = new List<OutputParameter>();
        public bool ShouldSerializeViewParameters() { return ViewParameters.Count > 0; }

        /// <summary>
        /// The view used to execute the report output
        /// </summary>
#if WINDOWS
        [Category("Definition"), DisplayName("View name"), Description("The view used to execute the report output."), Id(2, 1)]
        [TypeConverter(typeof(ReportViewConverter))]
#endif
        public string ViewGUID { get; set; }

        /// <summary>
        /// Path of the folder used to generate the report result.
        /// </summary>
#if WINDOWS
        [Category("Folder"), DisplayName("Folder path"), Description("Path of the folder used to generate the report result. The path can contain the keyword " + Repository.SealRepositoryKeyword + " to specify the repository root folder."), Id(2, 2)]
        [TypeConverter(typeof(RepositoryFolderConverter))]
#endif
        public string FolderPath { get; set; }


        public string FileServerFolderWithSeparators { 
            get
            {
                if (string.IsNullOrEmpty(FolderPath) || FolderPath == "/" || FolderPath == "\\") return "/";
                return (FolderPath.StartsWith("/") ? "" : "/") + FolderPath + (FolderPath.EndsWith("/") ? "" : "/");
            }
        }

        /// <summary>
        /// The name of the file used to generate the report result
        /// </summary>
#if WINDOWS
        [Category("Folder"), DisplayName("File name"), Description("The name of the file used to generate the report result. The file name can be formatted with the execution date time and can contain the keyword " + Repository.SealReportDisplayNameKeyword + " to specify the current report display name."), Id(3, 2)]
        [TypeConverter(typeof(CustomFormatConverter))]
#endif
        public string FileName { get; set; }

        /// <summary>
        /// The destination (To) email addresses used for the email. One per line or separated by semi-column.
        /// </summary>
#if WINDOWS
        [Category("Email Addresses"), DisplayName("TO"), Description("The destination (To) email addresses used for the email. One per line or separated by semi-column."), Id(2, 2)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
#endif
        public string EmailTo { get; set; }

        /// <summary>
        /// The email CC (Carbon Copy) addresses used for the email. One per line or separated by semi-column.
        /// </summary>
#if WINDOWS
        [Category("Email Addresses"), DisplayName("CC"), Description("The email CC (Carbon Copy) addresses used for the email. One per line or separated by semi-column."), Id(3, 2)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
#endif
        public string EmailCC { get; set; }

        /// <summary>
        /// The email BCC (Blind Carbon Copy) addresses used for the email. One per line or separated by semi-column.
        /// </summary>
#if WINDOWS
        [Category("Email Addresses"), DisplayName("BCC"), Description("The email BCC (Blind Carbon Copy) addresses used for the email. One per line or separated by semi-column."), Id(4, 2)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
#endif
        public string EmailBCC { get; set; }

        /// <summary>
        /// The email address of the sender (From). If empty, the sender email address defined in the device is used. Make sure that the email server allows the new address.
        /// </summary>
#if WINDOWS
        [Category("Email Addresses"), DisplayName("Sender"), Description("The email address of the sender (From). If empty, the sender email address defined in the device is used. Make sure that the email server allows the new address."), Id(11, 2)]
#endif
        public string EmailFrom { get; set; }

        /// <summary>
        /// The reply addresses used for the email. One per line or separated by semi-column. If empty, the reply addresses defined in the device are used.
        /// </summary>
#if WINDOWS
        [Category("Email Addresses"), DisplayName("Reply"), Description("The reply addresses used for the email. One per line or separated by semi-column. If empty, the reply addresses defined in the device are used."), Id(12, 2)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
#endif
        public string EmailReplyTo { get; set; }

        /// <summary>
        /// The subject of the email sent. If empty, the report name is used.
        /// </summary>
#if WINDOWS
        [Category("Email Subject"), DisplayName("Subject"), Description("The subject of the email sent. If empty, the report name is used."), Id(2, 3)]
#endif
        public string EmailSubject { get; set; }


        private bool _emailHtmlBody = false;
        /// <summary>
        /// If true, the report result is copied in the email body message
        /// </summary>
#if WINDOWS
        [DefaultValue(false)]
        [Category("Email Body"), DisplayName("Use HTML result for email body"), Description("If true, the report result is copied in the email body message."), Id(2, 4)]
#endif
        public bool EmailHtmlBody
        {
            get { return _emailHtmlBody; }
            set
            {
                _emailHtmlBody = value;
                UpdateEditorAttributes();
            }
        }

        private bool _emailMessagesInBody = false;
        /// <summary>
        /// If true, the report execution messages are copied in the email body message
        /// </summary>
#if WINDOWS
        [DefaultValue(false)]
        [Category("Email Body"), DisplayName("Use execution messages for email body"), Description("If true, the report execution messages are copied in the email body message."), Id(3, 4)]
#endif
        public bool EmailMessagesInBody
        {
            get { return _emailMessagesInBody; }
            set
            {
                _emailMessagesInBody = value;
                UpdateEditorAttributes();  
            }
        }

        /// <summary>
        /// The body message of the email sent. If empty, a default text is used.
        /// </summary>
#if WINDOWS
        [Category("Email Body"), DisplayName("Body message"), Description("The body message of the email sent. If empty, a default text is used."), Id(4, 4)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
#endif
        public string EmailBody { get; set; }

        private bool _emailSkipAttachments = false;
        /// <summary>
        /// If true, the email sent will have no attachement. This may be useful if the report has only tasks.
        /// </summary>
#if WINDOWS
        [DefaultValue(false)]
        [Category("Email Attachments"), DisplayName("Skip attachements"), Description("If true, the email sent will have no attachement. This may be useful if the report has only tasks."), Id(4, 5)]
#endif
        public bool EmailSkipAttachments
        {
            get { return _emailSkipAttachments; }
            set
            {
                _emailSkipAttachments = value;
                UpdateEditorAttributes();  
            }
        }

        private bool _zipResult = false;
        /// <summary>
        /// If true, the result file will be compressed in a zip file
        /// </summary>
#if WINDOWS
        [DefaultValue(false)]
        [Category("Zip options"), DisplayName("Zip result"), Description("If true, the result file will be compressed in a zip file."), Id(2, 5)]
#endif
        public bool ZipResult
        {
            get
            {

                return _zipResult;
            }
            set
            {
                _zipResult = value;
                UpdateEditorAttributes();  
            }
        }

        private string _zipPassword;
        /// <summary>
        /// If not empty, the Zip result file will be protected with the password
        /// </summary>
#if WINDOWS
        [Category("Zip options"), DisplayName("Zip password"), Description("If not empty, the Zip result file will be protected with the password."), Id(3, 5)]
#endif
        public string ZipPassword
        {
            get
            {
                return _zipPassword;
            }
            set
            {
                _zipPassword = value;
            }
        }

        /// <summary>
        /// If not empty, the output is generated with a security context having the name specified
        /// </summary>
#if WINDOWS
        [Category("Security and publication"), DisplayName("User name"), Description("If not empty, the output is generated with a security context having the name specified."), Id(1, 6)]
#endif
        public string UserName { get; set; }

        /// <summary>
        /// If not empty, the output is generated with a security context having the groups specified. One group name per line or separated by semi-column.
        /// </summary>
#if WINDOWS
        [Category("Security and publication"), DisplayName("User groups"), Description("If not empty, the output is generated with a security context having the groups specified. One group name per line or separated by semi-column."), Id(2, 6)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
#endif
        public string UserGroups { get; set; }

        /// <summary>
        /// The culture used to generate the report. If empty, the culture from the groups is used, then the default culture.
        /// </summary>
#if WINDOWS
        [Category("Security and publication"), DisplayName("Culture"), Description("The culture used to generate the report. If empty, the culture from the groups is used, then the default culture."), Id(3, 6)]
        [TypeConverter(typeof(Forms.CultureInfoConverter))]
#endif
        public string UserCulture { get; set; }

        /// <summary>
        /// For the Web Report Server: If true, the output can be executed by all users having the execute right on the report. If false, only the user owner can execute the schedule.
        /// </summary>
#if WINDOWS
        [DefaultValue(true)]
        [Category("Security and publication"), DisplayName("Public execution"), Description("For the Web Report Server: If true, the output can be executed by all users having the execute right on the report. If false, only the user owner can execute the schedule."), Id(4, 6)]
#endif
        public bool PublicExec { get; set; } = true;

        /// <summary>
        /// For the Web Report Server Designer: If true, the output and shedule can be edited by all users having the schedule right on the report. If false, only the user owner can edit the schedule.
        /// </summary>
#if WINDOWS
        [DefaultValue(true)]
        [Category("Security and publication"), DisplayName("Public edit"), Description("For the Web Report Server Designer: If true, the output and shedule can be edited by all users having the schedule right on the report. If false, only the user owner can edit the schedule."), Id(4, 6)]
#endif
        public bool PublicEdit { get; set; } = true;

        /// <summary>
        /// Object that can be used at run-time for any purpose
        /// </summary>
        [XmlIgnore]
        public object Tag;

        /// <summary>
        /// Format of the output
        /// </summary>
        [XmlIgnore]
        public ReportFormat Format
        {
            get
            {
                var param = ViewParameters.FirstOrDefault(i => i.Name == Parameter.ReportFormatParameter && i.CustomValue);
                if (param != null)
                {
                    var format = param.Value;
                    //from 8.2: converter not supported anymore -> formats to new formats
                    if (format == "excel") format = "Excel";
                    else if (format == "pdf") format = "PDF";

                    ReportFormat result;
                    if (Enum.TryParse(format, out result)) return result;
                }
                return Report.Format;
            }
            set
            {
                var param = ViewParameters.FirstOrDefault(i => i.Name == Parameter.ReportFormatParameter);
                if (param != null)
                {
                    param.CustomValue = true;
                    param.Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Current OutputDevice
        /// </summary>
        [XmlIgnore]
        public OutputDevice Device
        {
            get
            {
                return Report.Repository.Devices.FirstOrDefault(i => i.GUID == OutputDeviceGUID);
            }
        }

        /// <summary>
        /// Current View to execute
        /// </summary>
        [XmlIgnore]
        public ReportView View
        {
            get
            {
                if (Report != null) return Report.FindView(Report.Views, ViewGUID);
                return null;
            }
        }

        void SynchronizeRestrictions()
        {
            if (Report == null || !_useCustomRestrictions) return;

            List<ReportRestriction> allRestrictions = Report.AllRestrictions;

            //Check the existing restrictions
            int index = _restrictions.Count;
            while (--index >= 0)
            {
                ReportRestriction restriction = allRestrictions.FirstOrDefault(i => i.GUID == _restrictions[index].GUID);
                if (restriction == null) _restrictions.RemoveAt(index);
                else
                {
                    _restrictions[index].SetSourceReference(restriction.Source);
                    _restrictions[index].Report = Report;
                    _restrictions[index].Model = restriction.Model;
                }
            }

            //Add new restrictions
            foreach (var restriction in allRestrictions)
            {
                ReportRestriction outputRestriction = _restrictions.FirstOrDefault(i => i.GUID == restriction.GUID);
                if (outputRestriction == null)
                {
                    outputRestriction = ReportRestriction.CreateReportRestriction();
                    Helper.CopyProperties(restriction, outputRestriction);
                    _restrictions.Add(outputRestriction);
                }
                outputRestriction.PivotPosition = restriction.PivotPosition;
            }
        }

        bool _useCustomRestrictions = false;
        /// <summary>
        /// If true, custom restrictions can be defined for this output
        /// </summary>
#if WINDOWS
        [DefaultValue(false)]
        [Category("Restrictions"), DisplayName("Use Custom restrictions"), Description("If true, custom restrictions can be defined for this output."), Id(2, 6)]
#endif
        public bool UseCustomRestrictions
        {
            get { return _useCustomRestrictions; }
            set
            {
                _useCustomRestrictions = value;
                UpdateEditorAttributes();
            }
        }

        List<ReportRestriction> _restrictions = new List<ReportRestriction>();
        /// <summary>
        /// Custom restrictions applied to this output
        /// </summary>
#if WINDOWS
        [Category("Restrictions"), DisplayName("Custom restrictions"), Description("Custom restrictions applied to this output."), Id(3, 6)]
        [Editor(typeof(EntityCollectionEditor), typeof(UITypeEditor))]
#endif
        public List<ReportRestriction> Restrictions
        {
            get
            {
                SynchronizeRestrictions();
                return _restrictions;
            }
            set { _restrictions = value; }
        }
        public bool ShouldSerializeRestrictions() { return _restrictions.Count > 0; }

        #region Helpers
        /// <summary>
        /// Last information message
        /// </summary>
#if WINDOWS
        [Category("Helpers"), DisplayName("Information"), Description("Last information message."), Id(2, 20)]
        [Editor(typeof(InformationUITypeEditor), typeof(UITypeEditor))]
#endif
		[XmlIgnore]
        public string Information { get; set; }

        /// <summary>
        /// Last error message
        /// </summary>
#if WINDOWS
        [Category("Helpers"), DisplayName("Error"), Description("Last error message."), Id(3, 20)]
        [Editor(typeof(ErrorUITypeEditor), typeof(UITypeEditor))]
#endif
		[XmlIgnore]
        public string Error { get; set; }


        //Temporary variables to help for report serialization...
        private List<OutputParameter> _tempParameters;

        /// <summary>
        /// Function called before the serialization
        /// </summary>
        public void BeforeSerialization()
        {
            _tempParameters = ViewParameters.ToList();
            //Remove parameters not used
            ViewParameters.RemoveAll(i => !i.CustomValue);
        }

        /// <summary>
        /// Copy the current parameter values to a parameter list
        /// </summary>
        public void CopyParameters(List<Parameter> destination)
        {
            foreach (var parameter in ViewParameters.Where(i => i.CustomValue))
            {
                var destParameter = destination.FirstOrDefault(i => i.Name == parameter.Name);
                if (destParameter != null) destParameter.Value = parameter.Value;
            }
        }

        /// <summary>
        /// Function called after the serialization
        /// </summary>
        public void AfterSerialization()
        {
            ViewParameters = _tempParameters;
        }


        #endregion

    }


}

