# Localization Module

## Purpose

Provides multi-language support (Arabic, English, French) via JSON resource files with Redis caching and DB persistence.

## Architecture

```
Resources/
└── Translations/
    ├── ar.json    ← Arabic
    ├── en.json    ← English (default)
    └── fr.json    ← French

DB Tables:
├── Languages            # Language registry
├── LanguageResources    # Key-value translations
├── TranslationCaches    # Cached resolved values
└── LocalizationSettings # Per-tenant preferences
```

## Translation File Format

```json
{
  "invoice": "Invoice",
  "customer": "Customer",
  "total_amount": "Total Amount",
  "invoice_posted": "Invoice has been posted successfully",
  "validation.required": "This field is required"
}
```

Arabic (`ar.json`):
```json
{
  "invoice": "فاتورة",
  "customer": "عميل",
  "total_amount": "الإجمالي",
  "invoice_posted": "تم ترحيل الفاتورة بنجاح",
  "validation.required": "هذا الحقل مطلوب"
}
```

## Fallback Chain

```
User Language → Tenant Default → System Default (en) → Key itself
```

## Caching

Translations are cached in Redis for 24 hours:
```
Key: "localization:{languageCode}:{key}"
TTL: 24 hours
```

## API

```
GET /api/localization/languages          → List available languages
GET /api/localization/{lang}/keys        → All keys for a language
POST /api/localization/import            → Upload JSON file
GET /api/localization/stats              → Coverage statistics
```

## Adding a New Language

1. Create `Resources/Translations/{code}.json`
2. POST to `/api/localization/import` with the file
3. Cache auto-populates on first access
