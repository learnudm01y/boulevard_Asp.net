-- ============================================================
--  SERVICE FORM CONFIGURATION — Dynamic Field-Mapping System
--  Run against: BoulevardDb (.\SQLEXPRESS)
-- ============================================================

-- 1) Sections: groups of fields under a service
CREATE TABLE dbo.ServiceFormSections (
    SectionId          INT IDENTITY(1,1) PRIMARY KEY,
    FeatureCategoryId  INT           NOT NULL,          -- FK → FeatureCategories
    Title              NVARCHAR(200) NOT NULL,
    TitleAr            NVARCHAR(200) NULL,
    SortOrder          INT           NOT NULL DEFAULT 0,
    IsActive           BIT           NOT NULL DEFAULT 1,
    IsDelete           BIT           NOT NULL DEFAULT 0,
    CreateBy           INT           NOT NULL DEFAULT 1,
    CreateDate         DATETIME      NOT NULL DEFAULT GETDATE(),
    UpdateBy           INT           NULL,
    UpdateDate         DATETIME      NULL,

    CONSTRAINT FK_ServiceFormSections_FeatureCategory
        FOREIGN KEY (FeatureCategoryId)
        REFERENCES dbo.FeatureCategories (FeatureCategoryId)
);

-- 2) Fields: individual controls inside a section
CREATE TABLE dbo.ServiceFormFields (
    FieldId            INT IDENTITY(1,1) PRIMARY KEY,
    SectionId          INT           NOT NULL,          -- FK → ServiceFormSections
    FieldKey           NVARCHAR(100) NOT NULL,          -- internal dev key e.g. "full_name"
    Label              NVARCHAR(200) NOT NULL,          -- display label EN
    LabelAr            NVARCHAR(200) NULL,              -- display label AR
    Placeholder        NVARCHAR(300) NULL,
    PlaceholderAr      NVARCHAR(300) NULL,
    FieldType          NVARCHAR(50)  NOT NULL DEFAULT 'text',
        -- text | number | decimal | date | time | dropdown | radio
        -- checkbox | file | multiline | email | phone | url | hidden
    DataType           NVARCHAR(50)  NOT NULL DEFAULT 'string',
        -- string | int | decimal | bool | date | time | datetime | file
    IsRequired         BIT           NOT NULL DEFAULT 0,
    IsVisible          BIT           NOT NULL DEFAULT 1,
    SortOrder          INT           NOT NULL DEFAULT 0,
    DefaultValue       NVARCHAR(500) NULL,
    HelpText           NVARCHAR(500) NULL,
    HelpTextAr         NVARCHAR(500) NULL,
    ValidationRegex    NVARCHAR(500) NULL,              -- optional regex pattern
    MinLength          INT           NULL,
    MaxLength          INT           NULL,
    MinValue           NVARCHAR(100) NULL,              -- for number / date ranges
    MaxValue           NVARCHAR(100) NULL,
    IsDelete           BIT           NOT NULL DEFAULT 0,
    CreateBy           INT           NOT NULL DEFAULT 1,
    CreateDate         DATETIME      NOT NULL DEFAULT GETDATE(),
    UpdateBy           INT           NULL,
    UpdateDate         DATETIME      NULL,

    CONSTRAINT FK_ServiceFormFields_Section
        FOREIGN KEY (SectionId)
        REFERENCES dbo.ServiceFormSections (SectionId)
);

-- 3) Options: choices for dropdown / radio / checkbox fields
CREATE TABLE dbo.ServiceFormFieldOptions (
    OptionId           INT IDENTITY(1,1) PRIMARY KEY,
    FieldId            INT           NOT NULL,          -- FK → ServiceFormFields
    OptionLabel        NVARCHAR(200) NOT NULL,
    OptionLabelAr      NVARCHAR(200) NULL,
    OptionValue        NVARCHAR(200) NOT NULL,
    SortOrder          INT           NOT NULL DEFAULT 0,
    IsDefault          BIT           NOT NULL DEFAULT 0,
    IsDelete           BIT           NOT NULL DEFAULT 0,

    CONSTRAINT FK_ServiceFormFieldOptions_Field
        FOREIGN KEY (FieldId)
        REFERENCES dbo.ServiceFormFields (FieldId)
);

-- 4) Attachment rules: fine-grained file-upload config per field
CREATE TABLE dbo.ServiceFormAttachmentRules (
    AttachmentRuleId   INT IDENTITY(1,1) PRIMARY KEY,
    FieldId            INT           NOT NULL,          -- FK → ServiceFormFields (where FieldType = 'file')
    AllowedExtensions  NVARCHAR(500) NOT NULL DEFAULT '.pdf,.jpg,.jpeg,.png,.doc,.docx',
    MaxFileSizeMB      INT           NOT NULL DEFAULT 5,
    AllowMultiple      BIT           NOT NULL DEFAULT 0,
    IsRequired         BIT           NOT NULL DEFAULT 0,
    MaxFileCount       INT           NOT NULL DEFAULT 1,
    DisplayLabel       NVARCHAR(200) NULL,
    DisplayLabelAr     NVARCHAR(200) NULL,

    CONSTRAINT FK_ServiceFormAttachmentRules_Field
        FOREIGN KEY (FieldId)
        REFERENCES dbo.ServiceFormFields (FieldId)
);

-- ============================================================
--  SEED: Sample Typing service configuration
--  Typing FeatureCategoryId must be looked up from database.
-- ============================================================
DECLARE @typingFcId INT;
SELECT @typingFcId = FeatureCategoryId
  FROM dbo.FeatureCategories
 WHERE FeatureCategoryKey = 'f4309df5-9121-41ad-831a-994c46b62766';

-- Only seed if the typing category exists and no sections exist yet
IF @typingFcId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.ServiceFormSections WHERE FeatureCategoryId = @typingFcId)
BEGIN

    -- Section 1: Personal Information
    INSERT INTO dbo.ServiceFormSections (FeatureCategoryId, Title, TitleAr, SortOrder)
    VALUES (@typingFcId, N'Personal Information', N'المعلومات الشخصية', 1);
    DECLARE @sec1 INT = SCOPE_IDENTITY();

    INSERT INTO dbo.ServiceFormFields (SectionId, FieldKey, Label, LabelAr, Placeholder, PlaceholderAr, FieldType, DataType, IsRequired, SortOrder)
    VALUES
        (@sec1, 'full_name',    N'Full Name',    N'الاسم الكامل',    N'Enter your full name',      N'أدخل اسمك الكامل',     'text',  'string', 1, 1),
        (@sec1, 'email',        N'Email',        N'البريد الإلكتروني', N'email@example.com',       N'email@example.com',     'email', 'string', 1, 2),
        (@sec1, 'phone',        N'Phone Number', N'رقم الهاتف',      N'+971 50 000 0000',          N'+971 50 000 0000',      'phone', 'string', 1, 3),
        (@sec1, 'nationality',  N'Nationality',  N'الجنسية',         NULL,                         NULL,                     'dropdown', 'string', 1, 4);

    -- Options for nationality dropdown
    DECLARE @natFieldId INT;
    SELECT @natFieldId = FieldId FROM dbo.ServiceFormFields WHERE SectionId = @sec1 AND FieldKey = 'nationality';

    INSERT INTO dbo.ServiceFormFieldOptions (FieldId, OptionLabel, OptionLabelAr, OptionValue, SortOrder)
    VALUES
        (@natFieldId, N'UAE National', N'مواطن إماراتي', 'uae', 1),
        (@natFieldId, N'GCC National', N'مواطن خليجي', 'gcc', 2),
        (@natFieldId, N'Resident',     N'مقيم',         'resident', 3),
        (@natFieldId, N'Visitor',      N'زائر',         'visitor', 4);

    -- Section 2: Service Details
    INSERT INTO dbo.ServiceFormSections (FeatureCategoryId, Title, TitleAr, SortOrder)
    VALUES (@typingFcId, N'Service Details', N'تفاصيل الخدمة', 2);
    DECLARE @sec2 INT = SCOPE_IDENTITY();

    INSERT INTO dbo.ServiceFormFields (SectionId, FieldKey, Label, LabelAr, Placeholder, PlaceholderAr, FieldType, DataType, IsRequired, SortOrder, HelpText, HelpTextAr)
    VALUES
        (@sec2, 'service_type',    N'Service Type',    N'نوع الخدمة',      NULL, NULL, 'radio', 'string', 1, 1, N'Select the type of typing service you need', N'اختر نوع خدمة الطباعة التي تحتاجها'),
        (@sec2, 'urgency',         N'Urgency Level',   N'مستوى الاستعجال', NULL, NULL, 'dropdown', 'string', 0, 2, NULL, NULL),
        (@sec2, 'description',     N'Description',     N'الوصف',           N'Describe your request...', N'صف طلبك...', 'multiline', 'string', 0, 3, NULL, NULL),
        (@sec2, 'preferred_date',  N'Preferred Date',  N'التاريخ المفضل',  NULL, NULL, 'date', 'date', 0, 4, NULL, NULL);

    -- Options for service_type radio
    DECLARE @stFieldId INT;
    SELECT @stFieldId = FieldId FROM dbo.ServiceFormFields WHERE SectionId = @sec2 AND FieldKey = 'service_type';

    INSERT INTO dbo.ServiceFormFieldOptions (FieldId, OptionLabel, OptionLabelAr, OptionValue, SortOrder)
    VALUES
        (@stFieldId, N'Document Typing',     N'طباعة مستندات',    'document_typing', 1),
        (@stFieldId, N'Visa Processing',     N'معالجة التأشيرات', 'visa_processing', 2),
        (@stFieldId, N'PRO Services',        N'خدمات المندوب',    'pro_services', 3),
        (@stFieldId, N'Translation',         N'ترجمة',            'translation', 4);

    -- Options for urgency dropdown
    DECLARE @urgFieldId INT;
    SELECT @urgFieldId = FieldId FROM dbo.ServiceFormFields WHERE SectionId = @sec2 AND FieldKey = 'urgency';

    INSERT INTO dbo.ServiceFormFieldOptions (FieldId, OptionLabel, OptionLabelAr, OptionValue, SortOrder)
    VALUES
        (@urgFieldId, N'Normal',    N'عادي',     'normal', 1),
        (@urgFieldId, N'Urgent',    N'مستعجل',   'urgent', 2),
        (@urgFieldId, N'Express',   N'سريع جداً', 'express', 3);

    -- Section 3: Attachments
    INSERT INTO dbo.ServiceFormSections (FeatureCategoryId, Title, TitleAr, SortOrder)
    VALUES (@typingFcId, N'Required Documents', N'المستندات المطلوبة', 3);
    DECLARE @sec3 INT = SCOPE_IDENTITY();

    INSERT INTO dbo.ServiceFormFields (SectionId, FieldKey, Label, LabelAr, FieldType, DataType, IsRequired, SortOrder, HelpText, HelpTextAr)
    VALUES
        (@sec3, 'id_copy',       N'ID / Passport Copy',   N'صورة الهوية / جواز السفر', 'file', 'file', 1, 1, N'Upload a clear copy of your Emirates ID or passport', N'قم بتحميل صورة واضحة من هويتك الإماراتية أو جواز السفر'),
        (@sec3, 'supporting_docs', N'Supporting Documents', N'المستندات الداعمة',       'file', 'file', 0, 2, N'Upload any additional documents if needed',           N'قم بتحميل أي مستندات إضافية إذا لزم الأمر');

    -- Attachment rules
    DECLARE @idCopyFieldId INT, @suppDocsFieldId INT;
    SELECT @idCopyFieldId   = FieldId FROM dbo.ServiceFormFields WHERE SectionId = @sec3 AND FieldKey = 'id_copy';
    SELECT @suppDocsFieldId = FieldId FROM dbo.ServiceFormFields WHERE SectionId = @sec3 AND FieldKey = 'supporting_docs';

    INSERT INTO dbo.ServiceFormAttachmentRules (FieldId, AllowedExtensions, MaxFileSizeMB, AllowMultiple, IsRequired, MaxFileCount, DisplayLabel, DisplayLabelAr)
    VALUES
        (@idCopyFieldId,   '.pdf,.jpg,.jpeg,.png', 10, 0, 1, 1, N'Upload ID / Passport', N'تحميل الهوية / جواز السفر'),
        (@suppDocsFieldId, '.pdf,.jpg,.jpeg,.png,.doc,.docx,.xls,.xlsx', 10, 1, 0, 5, N'Upload Supporting Documents', N'تحميل المستندات الداعمة');

    -- Section 4: Agreement
    INSERT INTO dbo.ServiceFormSections (FeatureCategoryId, Title, TitleAr, SortOrder)
    VALUES (@typingFcId, N'Terms & Agreement', N'الشروط والموافقة', 4);
    DECLARE @sec4 INT = SCOPE_IDENTITY();

    INSERT INTO dbo.ServiceFormFields (SectionId, FieldKey, Label, LabelAr, FieldType, DataType, IsRequired, SortOrder, HelpText, HelpTextAr)
    VALUES
        (@sec4, 'agree_terms', N'I agree to the terms and conditions', N'أوافق على الشروط والأحكام', 'checkbox', 'bool', 1, 1, N'You must accept the terms to proceed', N'يجب عليك قبول الشروط للمتابعة');

    PRINT 'Typing service form configuration seeded successfully.';
END
ELSE
BEGIN
    PRINT 'Skipped seeding — Typing category not found or sections already exist.';
END
GO
