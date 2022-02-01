using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WorkQApi
{
    public class WorkqApi : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider) 
        {
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            string workqml = (string)context.InputParameters["workqxml"];//cr8d1_totalfees

            IOrganizationServiceFactory serviceFactory =
               (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(workqml); // suppose that myXmlString contains "<Names>...</Names>"

            
            try
            {
                XmlNodeList xnList = xml.SelectNodes("/root/person/workItemDetail");
                foreach (XmlNode xn in xnList)
                {
                    
                    Entity contact = new Entity("contact");
                    contact["firstname"] = xn["datafeed"].InnerText;
                    contact["lastname"] = xn["sourceId"].InnerText;
                    contact["address1_line1"] = xn["status"].InnerText;
                    Guid contactId = service.Create(contact);
                    Console.WriteLine("New contact id: {0}.", contactId.ToString());
                }

                


            }
            catch (Exception ex)
            {
                tracingService.Trace("CalculateRollupField Exception: {0}", ex.ToString());
                throw;
            }
        }
    }
}
