import axios from 'axios';

const baseURL = "https://localhost:7192/api/";

let globalSetLoading = null;
let requestCount = 0;

export const setLoaderCallback = (callback) => {
  globalSetLoading = callback;
};

axios.interceptors.request.use((config) => {
  requestCount++;
  if (globalSetLoading) globalSetLoading(true);
  return config;
}, (error) => {
  requestCount = Math.max(0, requestCount - 1);
  if (requestCount === 0 && globalSetLoading) globalSetLoading(false);
  return Promise.reject(error);
});

axios.interceptors.response.use((response) => {
  requestCount = Math.max(0, requestCount - 1);
  if (requestCount === 0 && globalSetLoading) globalSetLoading(false);
  return response;
}, (error) => {
  requestCount = Math.max(0, requestCount - 1);
  if (requestCount === 0 && globalSetLoading) globalSetLoading(false);
  return Promise.reject(error);
});

/**
 * Helper to get the default headers with or without an optional token.
 * Extracted here to avoid duplicating the Token logic across all methods.
 * 
 * @param {boolean} requiresAuth - Whether the endpoint requires authentication.
 * @param {string|null} customToken - An optional custom token to use instead of the one in localStorage.
 * @returns {object} Headers configuration object.
 */
const getHeaders = (requiresAuth = true, customToken = null, body = null) => {
  const headers = {
    'Content-Type': 'application/json',
  };

  if (body instanceof FormData) {
    delete headers['Content-Type'];
  }

  if (requiresAuth) {
    // If a custom token is provided, use it; otherwise fallback to localStorage
    const token = customToken || localStorage.getItem("crime_system_token");
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
  }

  return headers;
};

/**
 * Reusable error handler to keep catch blocks clean.
 * @param {object} error - The Axios error object.
 * @returns {object} A consistently formatted error response.
 */
const handleError = (error) => {
  if (error.response) {
    // The request was made and the server responded with a status code outside the range of 2xx
    return {
      success: false,
      message: error.response.data?.message || `Request failed with status ${error.response.status}`,
      errors: error.response.data?.errors || null,
      status: error.response.status
    };
  } else if (error.request) {
    // The request was made but no response was received
    return {
      success: false,
      message: "No response received from the server. Please check your network connection."
    };
  } else {
    // Something happened in setting up the request that triggered an Error
    return {
      success: false,
      message: error.message || "An unexpected error occurred."
    };
  }
};

/**
 * Make a POST request WITHOUT token authentication.
 * Use for public endpoints like Login or basic Registration.
 * @param {string} apiName - The API endpoint path.
 * @param {object} postData - The data payload to send.
 */
const PostServiceCall = async (apiName, postData) => {
  try {
    const response = await axios.post(`${baseURL}${apiName}`, postData, {
      headers: getHeaders(false)
    });
    return response.data;
  } catch (error) {
    return handleError(error); // Note: if you prefer rejecting the promise, throw handleError(error)
  }
};

/**
 * Make a POST request WITH token authentication.
 * Use for secured endpoints.
 * @param {string} apiName - The API endpoint path.
 * @param {object} postData - The data payload to send.
 */
const PostServiceCallToken = async (apiName, postData) => {
  try {
    const response = await axios.post(`${baseURL}${apiName}`, postData, {
      headers: getHeaders(true, null, postData)
    });
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

/**
 * Make a POST request with a explicitly passed token.
 * Useful when working with refresh tokens or multiple active tokens.
 * @param {string} apiName - The API endpoint path.
 * @param {object} postData - The data payload to send.
 * @param {string} token - The manual JWT token to use.
 */
const PostServiceCallTokenWithToken = async (apiName, postData, token) => {
  try {
    const response = await axios.post(`${baseURL}${apiName}`, postData, {
      headers: getHeaders(true, token, postData)
    });
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

/**
 * Make a GET request WITH token authentication.
 * Use for retrieving secured information.
 * @param {string} apiName - The API endpoint path.
 */
const GetServiceCall = async (apiName) => {
  try {
    const response = await axios.get(`${baseURL}${apiName}`, {
      headers: getHeaders(true)
    });
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

/**
 * Make a GET request WITHOUT token authentication.
 * Use for public info endpoints.
 * @param {string} apiName - The API endpoint path.
 */
const GetService = async (apiName) => {
  try {
    const response = await axios.get(`${baseURL}${apiName}`, {
      headers: getHeaders(false)
    });
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

/**
 * Make a GET request with an explicitly provided token.
 * @param {string} apiName - The API endpoint path.
 * @param {string} token - The manual JWT token to use.
 */
const GetServiceCallWithToken = async (apiName, token) => {
  try {
    const response = await axios.get(`${baseURL}${apiName}`, {
      headers: getHeaders(true, token)
    });
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

/**
 * Make a DELETE request WITH token authentication.
 * @param {string} apiName - The API endpoint path.
 */
const Deleteserivecall = async (apiName) => {
  try {
    const response = await axios.delete(`${baseURL}${apiName}`, {
      headers: getHeaders(true)
    });
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

/**
 * Make a GET request to a complete third-party URL (ignores the default base URL).
 * @param {string} apiUrl - The full absolute URL of the third-party API.
 */
const ApiCallThirdParty = async (apiUrl) => {
  try {
    // Note: No authorization token headers are attached here since it's a 3rd-party endpoint.
    const response = await axios.get(apiUrl, {
      headers: { 'Content-Type': 'application/json' }
    });
    return response.data;
  } catch (error) {
    return handleError(error);
  }
};

// Bundle into a single service object 
const AuthService = {
  PostServiceCallToken,
  PostServiceCall,
  GetServiceCall,
  GetService,
  Deleteserivecall,
  ApiCallThirdParty,
  GetServiceCallWithToken,
  PostServiceCallTokenWithToken
};

export default AuthService;
