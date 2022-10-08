using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

using MySql.EntityFrameworkCore.Extensions;

namespace haku_chat.Common.DataBase
{
    public class MysqlEntityFrameworkDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkMySQL();
            new EntityFrameworkRelationalDesignServicesBuilder(serviceCollection)
                .TryAddCoreServices();
        }
    }
}
