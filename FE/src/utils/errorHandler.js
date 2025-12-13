/**
 * Utility functions for error handling and logging
 */

/**
 * Extract error message from error object
 * @param {Error|Object} error - Error object from catch block
 * @param {string} defaultMessage - Default message if error message cannot be extracted
 * @returns {string} - User-friendly error message
 */
export const getErrorMessage = (error, defaultMessage = 'Đã xảy ra lỗi') => {
  if (!error) return defaultMessage

  // If error is already a string
  if (typeof error === 'string') return error

  // Check for response data (Axios errors)
  if (error.response?.data) {
    const data = error.response.data

    // Check for message property
    if (data.message) return data.message

    // Check for title property
    if (data.title) return data.title

    // Check for errors object (validation errors)
    if (data.errors) {
      if (Array.isArray(data.errors)) {
        return data.errors.join(', ')
      }
      if (typeof data.errors === 'object') {
        const errorMessages = Object.values(data.errors).flat()
        return errorMessages.join(', ')
      }
    }

    // If data is a string
    if (typeof data === 'string') return data
  }

  // Check for error message
  if (error.message) return error.message

  // Fallback to default message
  return defaultMessage
}

/**
 * Get error message based on HTTP status code
 * @param {number} status - HTTP status code
 * @returns {string} - User-friendly error message
 */
export const getErrorByStatus = (status) => {
  const statusMessages = {
    400: 'Yêu cầu không hợp lệ',
    401: 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.',
    403: 'Bạn không có quyền thực hiện thao tác này',
    404: 'Không tìm thấy dữ liệu',
    500: 'Lỗi server. Vui lòng thử lại sau hoặc liên hệ quản trị viên.',
    502: 'Lỗi kết nối đến server',
    503: 'Server đang bảo trì',
  }

  return statusMessages[status] || 'Đã xảy ra lỗi không xác định'
}

/**
 * Log error for debugging (only in development)
 * @param {Error|Object} error - Error object
 * @param {string} context - Context where error occurred
 */
export const logError = (error, context = '') => {
  // Only log in development
  if (process.env.NODE_ENV === 'development') {
    const contextMsg = context ? `[${context}]` : ''
    console.error(`${contextMsg} Error:`, error)
    
    // Log additional details if available
    if (error?.response?.data) {
      console.error(`${contextMsg} Response data:`, error.response.data)
    }
    if (error?.response?.status) {
      console.error(`${contextMsg} Status:`, error.response.status)
    }
  }
}

/**
 * Handle API error and return user-friendly message
 * @param {Error|Object} error - Error object from catch block
 * @param {string} defaultMessage - Default message
 * @param {string} context - Context for logging
 * @returns {string} - User-friendly error message
 */
export const handleApiError = (error, defaultMessage = 'Đã xảy ra lỗi', context = '') => {
  logError(error, context)
  
  // Try to get status-specific message first
  if (error?.response?.status) {
    const statusMessage = getErrorByStatus(error.response.status)
    if (statusMessage) return statusMessage
  }

  // Then try to extract message from error
  return getErrorMessage(error, defaultMessage)
}

