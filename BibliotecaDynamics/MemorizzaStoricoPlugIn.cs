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
                        if (context.Stage == 40) //40 is postoperation
                        {

                        }
                        var queryexpressione = new QueryExpression(context.PrimaryEntityName);
                        // queryexpressione.
                        var result = svc.Retrieve(context.PrimaryEntityName, libro, new ColumnSet(true)); //non prendere mai tutte le colonne
                        result.GetAttributeValue<EntityReference>("nomeeeeCampo");

                        var entity = new Entity("crb92_librocliente");
                        entity.Attributes["crb92_nomeproprietario"] = "testNuovoPlugIn";
                        entity.Attributes["crb92_libro"] = "nmBibliotecaNuova";
                        entity.Attributes["crb92_name"] = "libro_clienteBBnuova";
                        svc.Create(entity); //create a new record for entity
                        return;
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
