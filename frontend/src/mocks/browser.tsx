// src/mocks/browser.ts
import { setupWorker } from 'msw/browser';
import { handlers } from './handlers';

// This will register a Service Worker that uses your `handlers`
export const worker = setupWorker(...handlers);
