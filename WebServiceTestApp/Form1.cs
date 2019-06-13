using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebServiceTestApp.com.dhl.wsbexpress.ratebook;

namespace WebServiceTestApp
{
    public partial class Form1 : Form
    {
        private bool IsProduction;
        private string productionBaseUrl = "https://wsbexpress.dhl.com/gbl/";
        private string testingBaseUrl = "https://wsbexpress.dhl.com/sndpt/";


        public Form1()
        {
            InitializeComponent();
        }

        public Tuple<EndpointAddress, BindingElementCollection, string, string> PrepareGlowsAuth(string endpoint)
        {
            EndpointAddress soapEndpoint = new EndpointAddress(string.Format("{0}/{1}", (IsProduction ? productionBaseUrl : testingBaseUrl), endpoint));
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.TransportWithMessageCredential;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

            BindingElementCollection elements = binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().EnableUnsecuredResponse = true;

            return new Tuple<EndpointAddress, BindingElementCollection, string, string>(soapEndpoint, elements, "username", "password");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // Assume that you named your "Connected Service" com.example.foo

            getRateRequestRequest requestObj = new getRateRequestRequest();

            // Fill in your request object
            DateTime timestamp = DateTime.Now.AddMinutes(10);

            requestObj.RequestedShipment = new docTypeRef_RequestedShipmentType2();
            requestObj.RequestedShipment.ShipTimestamp = $"{timestamp:s} GMT{timestamp:zzz}";
            // etc.

            // Set up the authentication using the function you provided
            var glowsAuthData = PrepareGlowsAuth("expressRateBook");

            // foo.<object name>Client is automatically created, this is the generated
            //   proxy class for communicating with the intended web service
            gblExpressRateBookClient client = new gblExpressRateBookClient(new CustomBinding(glowsAuthData.Item2)
                                                     , glowsAuthData.Item1);
            client.ClientCredentials.UserName.UserName = glowsAuthData.Item3;
            client.ClientCredentials.UserName.Password = glowsAuthData.Item4;

            // Use the client to send the request object and populate the response object
            // foo.<object name>Response is automatically generated when VS generates 
            //   the code for "Connected Service". It also makes it the return type 
            //   for foo.barClient.barResponse(foo.bar);
            getRateRequestResponse responseObj = client.getRateRequest(requestObj);
        }
    }
}
