using Api_Project.Models;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api_Project.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class Controller : ControllerBase
    {
        private static SecretClient _client = new SecretClient(vaultUri: new Uri("https://myserverstatuskeyvault12.vault.azure.net/"), credential: new DefaultAzureCredential());
        private static KeyVaultSecret _secret = _client.GetSecret("ApiKeyThingy");
        private static CosmosClient _cosmosDB = new CosmosClient(@"https://apicosmosdb12.documents.azure.com:443/", _secret.Value);
        private static Microsoft.Azure.Cosmos.Container _Results = _cosmosDB.GetContainer("ApiStorage", "Results");
        private readonly ILogger<Controller> _logger;
        public Controller(ILogger<Controller> logger)
        {
            _logger = logger;
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseResult>> Get(string id)
        {
            try
            {
                StorageObject readItem = await _Results.ReadItemAsync<StorageObject>(
               id: id,
               partitionKey: new PartitionKey(id));
                return readItem.result;
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<ValuesController>
        [HttpPost("{id}/{Height}/{Weight}/{Age}/{Gender}/{Workouts_Per_Week}")]
        public async Task<ActionResult<ResponseResult>> Post(string id, string Height, double Weight, int Age, string Gender,int Workouts_Per_Week)
        {
            try
            {
                var item = new StorageObject();
                item.id = id;
                item.person = new Person();
                item.person.Height = Height;
                item.person.Weight = Weight;
                item.person.Age = Age;
                item.person.Gender = Gender;
                item.person.Workouts_Per_Week = Workouts_Per_Week;

                item.result = item.CalculateResults(id, item.person);
                await _Results.CreateItemAsync<StorageObject>(item, new PartitionKey(id));

                return item.result;
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<StorageObject>> Put(string id, string? Height = null, double Weight = 0, int Age=0, string? Gender=null, int Workouts_Per_Week=0)
        {
            try
            {
                StorageObject readItem = await _Results.ReadItemAsync<StorageObject>(
               id: id,
               partitionKey: new PartitionKey(id));
                if (Height != null)
                {
                    readItem.person.Height = Height;
                }

                if (Weight != 0)
                {
                    readItem.person.Weight = Weight;
                }
                if (Age != 0)
                {
                    readItem.person.Age = Age;
                }
                if (Gender != null)
                {
                    readItem.person.Gender = Gender;
                }
                if (Workouts_Per_Week != 0)
                {
                    readItem.person.Workouts_Per_Week = Workouts_Per_Week;
                }

                readItem.result = readItem.CalculateResults(id, readItem.person);
                await _Results.ReplaceItemAsync<StorageObject>(readItem, id, new PartitionKey(id));
                return readItem;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _Results.DeleteItemAsync<StorageObject>(id, new PartitionKey(id));
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


    }
}
