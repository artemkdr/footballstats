import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import i18n from 'i18next';
import Backend from 'i18next-http-backend';

export const supportedLngs = ['en'];

i18n
.use(Backend)
.use(LanguageDetector)
.use(initReactI18next)
.init({
  detection: { 
    order: ['querystring', 'localStorage', 'navigator'],
    lookupQuerystring: 'lang',
  },
  backend: {
    loadPath: '/locales/{{lng}}.json', // Path to load translation files
  },
  interpolation: {
    escapeValue: false, // React already does escaping
    format: (value, format, lng) => {
      if (format === 'deApostrophe' && lng === "fr") {        
        return (/[aeiouy]/i.test(value?.charAt(0).normalize("NFD").replace(/[\u0300-\u036f]/g, "")) ? "d'" : "de ") + value;
      }
      return value; // Default behavior for other formats
    }
  },
  fallbackLng: 'en',
  supportedLngs: supportedLngs
});

export default i18n;