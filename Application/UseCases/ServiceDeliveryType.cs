using Application.DTOs.Response;
using Application.Interfaces.Query;
using Application.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class ServiceDeliveryType : IServiceDeliveryType
    {
        private readonly IDeliveryTypeQuery _deliveryTypeQuery;

        public ServiceDeliveryType(IDeliveryTypeQuery deliveryTypeQuery)
        {
            _deliveryTypeQuery = deliveryTypeQuery;
        }
        public async Task<List<GenericResponse>> GetDeliveryTypes()
        {
            var deliveryTypes = await _deliveryTypeQuery.GetListDeliveryTypeAsync();
            var deliveryTypeResponse = new List<GenericResponse>();

            foreach (var type in deliveryTypes)
            {
                var deliveryTypeDto = new GenericResponse
                {
                    id = type.Id,
                    name = type.Name
                };
                deliveryTypeResponse.Add(deliveryTypeDto);
            }
            return deliveryTypeResponse;

        }
    }
}
