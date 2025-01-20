using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Reports.CustomReports.Services
{
    public class CustomWorkflowMessageService : WorkflowMessageService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;   
        private readonly IStoreContext _storeContext;
        private readonly ITokenizer _tokenizer;
        public CustomWorkflowMessageService(
            CommonSettings commonSettings,
            EmailAccountSettings emailAccountSettings,
            IAddressService addressService,
            IAffiliateService affiliateService,
            ICustomerService customerService,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILogger logger,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            IOrderService orderService,
            IProductService productService,
            IQueuedEmailService queuedEmailService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITokenizer tokenizer)
            : base(
            commonSettings,
            emailAccountSettings,
            addressService,
            affiliateService,
            customerService,
            emailAccountService,
            eventPublisher,
            languageService,
            localizationService,
            messageTemplateService,
            messageTokenProvider,
            orderService,
            productService,
            queuedEmailService,
            storeContext,
            storeService,
            tokenizer)
        {
            _eventPublisher = eventPublisher;
            _messageTokenProvider = messageTokenProvider;
            _localizationService = localizationService;
            _logger = logger;
            _storeContext = storeContext;
            _tokenizer = tokenizer;
        }

        public async Task<IList<int>> SendCustomReportEmailAsync(string recipientEmail, string attachmentFilePath = null)
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var languageId = 2;

            var messageTemplates = await GetActiveMessageTemplatesAsync("Reports.ExcelEmail", currentStore.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var tokens = new List<Token>
            {
                new Token("RecipientEmail", recipientEmail),
                new Token("ReportName", recipientEmail),
                new Token("Date", DateTime.UtcNow.ToString("yyyy-MM-dd"))
            };

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                await _messageTokenProvider.AddStoreTokensAsync(tokens, currentStore, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var toEmail = recipientEmail;
                var toName = "Na ki";

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName, attachmentFilePath);
            }).ToListAsync();
        }
    }
}
