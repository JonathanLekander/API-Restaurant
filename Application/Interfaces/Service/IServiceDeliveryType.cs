using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IServiceDeliveryType
    {
        Task<List<GenericResponse>> GetDeliveryTypes();
    }
}
