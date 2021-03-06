using Microsoft.AspNetCore.Mvc;
using Petclinic.Vets.DTOs;
using Petclinic.Vets.Infrastructure.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace Petclinic.Vets.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class VetsController : ControllerBase
    {
        private readonly IVets _vetsRepo;
        private readonly IVetSpecialties _vetSpecialtiesRepo;

        public VetsController(IVets vetsRepo, IVetSpecialties vetSpecialtiesRepo)
        {
            _vetsRepo = vetsRepo;
            _vetSpecialtiesRepo = vetSpecialtiesRepo;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<VetDetails>), (int)HttpStatusCode.OK)]
        public ActionResult<List<VetDetails>> ShowResourcesVetList(CancellationToken cancellationToken)
        {
            var ret = new List<VetDetails>();

            _vetsRepo.FindAll().ToList().ForEach((vet) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var vetSpecialties = _vetSpecialtiesRepo.FindAllByVetId(vet.Id);

                var specialtyDetails = new List<SpecialtyDetails>();
                vetSpecialties.ToList().ForEach((vetSpecialty) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    specialtyDetails.Add(new SpecialtyDetails(vetSpecialty.Specialty.Id, vetSpecialty.Specialty.Name));
                });

                ret.Add(new VetDetails(vet.Id, vet.FirstName, vet.LastName, specialtyDetails));
            });

            return Ok(ret);
        }
    }
}
