export interface Language {
  code: string;
  name: string;
  isDefault?: boolean;
  isEnabled?: boolean;
  direction?: 'ltr' | 'rtl';
  flagIcon?: string;
  translationProgress?: number;
  lastUpdated?: Date;
}