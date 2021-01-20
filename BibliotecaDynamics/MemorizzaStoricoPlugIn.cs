using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaDynamics
{
    public class MemorizzaStoricoPlugIn : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //extract the tracing service for use in debugging  sandbox plugin
            //If you are not  registering the plugin in the sandbox, then you do ù
            //not have to add tracing service related code. 
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            //obtain the execution context from the serviceProvider
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));


            if (context.MessageName != "Update")
            {
                return;
            }



            //the InputParameter collection contains all the data passed in the message request 
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                //obtain the target identity from the input parameters
                Entity ent = (Entity)context.InputParameters["Target"];


                //verify that the target entity represents an entity type you are expeting
                if (ent.LogicalName == "crb92_libro2")
                {
                    var libro = context.PrimaryEntityId; //id del libro che sto modificado (a livello di record)

                    IOrganizationServiceFactory svcFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService svc = svcFactory.CreateOrganizationService(null); //con null diventa system

                    try
                    {
                        //business logic here

                        //prendo il valore prima
                        Entity preMessageImage;
                        preMessageImage = (Entity)context.PreEntityImages["preImage"];
                        string proprietarioOld = ((Microsoft.Xrm.Sdk.EntityReference)preMessageImage.Attributes["crb92_cliente2"]).Name;
                        string nomeLibro = preMessageImage.Attributes["crb92_name"].ToString();

                        Entity postMessageImage;
                        postMessageImage = (Entity)context.PostEntityImages["postImage"];
                        string proprietarioNew = ((Microsoft.Xrm.Sdk.EntityReference)postMessageImage.Attributes["crb92_cliente2"]).Name;
                        var image = context.InputParameters;

                        if (context.Stage == 40) //40 is postoperation
                        {


                            var queryexpressione = new QueryExpression(context.PrimaryEntityName);
                            // queryexpressione.
                            var result = svc.Retrieve(context.PrimaryEntityName, libro, new ColumnSet(true)); //non prendere mai tutte le colonne
                            result.GetAttributeValue<EntityReference>("nomeCampo2");

                            var entity = new Entity("crb92_librocliente");
                            entity.Attributes["crb92_nomeproprietario"] = proprietarioNew;
                            entity.Attributes["crb92_libro"] = nomeLibro;
                            entity.Attributes["crb92_name"] = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "#" + proprietarioNew + "#" + nomeLibro + "#inizioPrestito#";
                            svc.Create(entity); //create a new record for entity

                            entity = new Entity("crb92_librocliente");
                            entity.Attributes["crb92_nomeproprietario"] = proprietarioOld;
                            entity.Attributes["crb92_libro"] = nomeLibro;
                            entity.Attributes["crb92_name"] = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")+"#"+proprietarioOld + "#"+ nomeLibro + "#finePrestito#" ;
                            svc.Create(entity); //create a new record for entity
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        tracingService.Trace("MemorizzaStoricoPlugIn: {0}", ex.ToString());
                        throw;
                    }


                }
                else
                {
                    return;
                }
            }


        }
    }
}
