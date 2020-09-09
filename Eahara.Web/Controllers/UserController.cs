using Eahara.BL;
using Eahara.Model;
using Eahara.Web.Dtos;
using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        [HttpGet]
        [Route("GetAllUsers")]
        public List<UserDto> GetAllUsers()
        {
            using (EAharaDB context = new EAharaDB())
            {
                var datas = context.Users.Where(x => x.IsActive && !x.IsBlocked && x.Role != "0").OrderByDescending(x => x.Id)

                    .Select(x => new UserDto
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        Type = x.Type,
                    }).ToList();

                return datas;
            }
        }

        [HttpPost]
        [Route("AddDevice")]
        public bool AddDevice(DeviceDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {

                    Device device = new Device();

                    device.Platform = dataDto.Platform;
                    device.UUId = dataDto.UUId;
                    device.version = dataDto.version;
                    device.Language = dataDto.Language;

                    context.Devices.Add(device);

                    context.SaveChanges();
                    return true;

                }
            }
            return false;
        }

        [HttpGet]
        [Route("DeviceByIdUID/{Uid}")]
        public DeviceDto DeviceByIdUID(string uuid)
        {
            DeviceDto dataDto = new DeviceDto();
            using (EAharaDB context = new EAharaDB())
            {
                var type = context.Devices.FirstOrDefault(x => x.UUId == uuid);

                dataDto.Platform = type.Platform;
                dataDto.UUId = type.UUId;
                dataDto.version = type.version;
                dataDto.Language = type.Language;

                return dataDto;
            }
        }

        [HttpPost]
        [Route("Addusers")]
        public bool Addusers(UserDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (dataDto.Id > 0)
                    {
                        var data = context.Users.FirstOrDefault(x => x.Id == dataDto.Id);
                        if (data != null)
                        {
                            data.UserName = dataDto.UserName;
                            data.ShopId = dataDto.ShopId;
                            data.EmployeeId = dataDto.EmployeeId;
                            data.MEDShopId = dataDto.MEDShopId;
                            data.Role = dataDto.Role;

                            if (dataDto.IsNotSkip != true)
                            {
                                var passwordSalt = AuthenticationBL.CreatePasswordSalt(Encoding.ASCII.GetBytes(dataDto.Password));
                                data.PasswordSalt = Convert.ToBase64String(passwordSalt);
                                var password = AuthenticationBL.CreateSaltedPassword(passwordSalt, Encoding.ASCII.GetBytes(dataDto.Password));
                                data.Password = Convert.ToBase64String(password);
                            }

                            context.Entry(data).Property(x => x.UserName).IsModified = true;
                            context.Entry(data).Property(x => x.Password).IsModified = true;
                            context.Entry(data).Property(x => x.PasswordSalt).IsModified = true;
                            context.Entry(data).Property(x => x.ShopId).IsModified = true;
                            context.Entry(data).Property(x => x.EmployeeId).IsModified = true;
                            context.Entry(data).Property(x => x.Role).IsModified = true;                          
                            context.Entry(data).Property(x => x.MEDShopId).IsModified = true;                          

                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {

                        var olduser = context.Users.FirstOrDefault(x => x.IsActive && x.UserName == dataDto.UserName);
                        if(olduser != null)
                        {
                            return false;
                        }

                        User user = new User();

                        user.UserName = dataDto.UserName;
                        user.ShopId = dataDto.ShopId;
                        user.EmployeeId = dataDto.EmployeeId;
                        user.MEDShopId = dataDto.MEDShopId;
                        user.Role = dataDto.Role;
                        user.IsActive = true;

                        var passwordSalt = AuthenticationBL.CreatePasswordSalt(Encoding.ASCII.GetBytes(dataDto.Password));
                        user.PasswordSalt = Convert.ToBase64String(passwordSalt);
                        var password = AuthenticationBL.CreateSaltedPassword(passwordSalt, Encoding.ASCII.GetBytes(dataDto.Password));
                        user.Password = Convert.ToBase64String(password);

                        context.Users.Add(user);
                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }

        [HttpGet]
        [Route("DeleteUsers/{id}")]
        public bool DeleteUsers(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Users.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.IsActive = false;
                        context.Entry(Delete).Property(x => x.IsActive).IsModified = true;
                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpPost]
        [Route("UsersInView")]
        public DataSourceResult OffersInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var  query = context.Users.Where(x => x.IsActive == true);
                    query = query.Where(x => x.Role != "Customer");
                    var dataSourceResult = query
                    .Select(x => new UserDto
                    {

                        Id = x.Id,
                        UserName = x.UserName,
                        Password = x.Password,
                        ShopId = x.ShopId,
                        EmployeeId = x.EmployeeId,
                        MEDShopId = x.MEDShopId,
                        Role = x.Role,
                        IsActive = x.IsActive,
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

    }
}
