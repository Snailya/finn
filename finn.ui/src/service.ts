export const apiFetch = (url: string, init?: RequestInit | undefined) =>
  url.startsWith("/")
    ? fetch(`${process.env.REACT_APP_BACKEND_URL}${url}`, init)
    : fetch(url, init);
