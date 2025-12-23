using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Controllers.Interfaces;
using MortgageComparer.Services.Interfaces;
using System.Collections.Generic;

namespace MortgageComparer.Controllers {

    [Route("api/config/job-types")]
    public class JobTypesController : Controller, IReadController<List<JobTypeEntity>, JobTypeEntity, int> {


        private readonly IJobTypeService _jobTypeService;
        public JobTypesController(IJobTypeService jobTypeService) {
            _jobTypeService = jobTypeService;
        }
        public Task<ActionResult<IEnumerable<List<JobTypeEntity>>>> GetAll() {
            throw new NotImplementedException();
        }

        public Task<ActionResult<JobTypeEntity>> GetById([FromRoute] int id) {
            throw new NotImplementedException();
        }
    }

    public class DocumentTypesController : Controller, IReadController<List<DocumentTypeEntity>, DocumentTypeEntity, int> {
        private readonly IDocumentService _documentService;

        public DocumentTypesController(IDocumentService documentService) {
            _documentService = documentService;
        }
        public Task<ActionResult<IEnumerable<List<DocumentTypeEntity>>>> GetAll() {
            throw new NotImplementedException();
        }

        public Task<ActionResult<DocumentTypeEntity>> GetById([FromRoute] int id) {
            throw new NotImplementedException();
        }
    }

    public class QuoteController : Controller, IReadController<List<QuoteEntity>, QuoteEntity, int>, ICreateController {
        private readonly IQuoteService _quoteService;
        public QuoteController(IQuoteService quoteService) {
            _quoteService = quoteService;
        }

        public Task<ActionResult> AddAsync([FromBody] object? dto) {
            throw new NotImplementedException();
        }

        public Task<ActionResult<IEnumerable<List<QuoteEntity>>>> GetAll() {
            throw new NotImplementedException();
        }
        public Task<ActionResult<QuoteEntity>> GetById([FromRoute] int id) {
            throw new NotImplementedException();
        }
    }

    public class OfferController : Controller, IReadController<List<OfferEntity>, OfferEntity, int>, ICreateController{
        private readonly IOfferService _offerService;
        public OfferController(IOfferService offerService) {
            _offerService = offerService;
        }

        public Task<ActionResult> AddAsync([FromBody] object? dto) {
            throw new NotImplementedException();
        }

        public Task<ActionResult<IEnumerable<List<OfferEntity>>>> GetAll() {
            throw new NotImplementedException();
        }
        public Task<ActionResult<OfferEntity>> GetById([FromRoute] int id) {
            throw new NotImplementedException();
        }
    }

    public class DocumentController : Controller, IReadController<List<DocumentEntity>, DocumentEntity, int>, ICreateController{
        private readonly IDocumentService _documentService;
        public DocumentController(IDocumentService documentService) {
            _documentService = documentService;
        }

        public Task<ActionResult> AddAsync([FromBody] object? dto) {
            throw new NotImplementedException();
        }

        public Task<ActionResult<IEnumerable<List<DocumentEntity>>>> GetAll() {
            throw new NotImplementedException();
        }
        public Task<ActionResult<DocumentEntity>> GetById([FromRoute] int id) {
            throw new NotImplementedException();
        }
    }

}