import { MyConfig } from '../my-config';

export function resolveApiAssetUrl(path?: string | null, fallback: string = ''): string {
  if (!path) {
    return fallback;
  }

  if (/^(https?:|data:|blob:)/i.test(path)) {
    return path;
  }

  const normalized = path.startsWith('/') ? path : `/${path}`;
  const isFrontendAsset = normalized.startsWith('/images/defaul') || normalized.startsWith('/assets/');
  if (isFrontendAsset) {
    return normalized;
  }

  return `${MyConfig.api_address}${normalized}`;
}
