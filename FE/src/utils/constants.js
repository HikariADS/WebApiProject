/**
 * Constants used throughout the application
 */

export const ROLES = {
  ADMIN: 'Admin',
  MANAGER: 'Manager',
  USER: 'User',
}

export const ERROR_MESSAGES = {
  FETCH_FAILED: 'Không thể tải dữ liệu',
  SAVE_FAILED: 'Không thể lưu dữ liệu',
  DELETE_FAILED: 'Không thể xóa dữ liệu',
  UPDATE_FAILED: 'Không thể cập nhật dữ liệu',
  NETWORK_ERROR: 'Lỗi kết nối mạng. Vui lòng kiểm tra kết nối của bạn.',
  UNAUTHORIZED: 'Bạn không có quyền thực hiện thao tác này',
  SESSION_EXPIRED: 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.',
  SERVER_ERROR: 'Lỗi server. Vui lòng thử lại sau hoặc liên hệ quản trị viên.',
}

export const SUCCESS_MESSAGES = {
  SAVED: 'Đã lưu thành công',
  UPDATED: 'Đã cập nhật thành công',
  DELETED: 'Đã xóa thành công',
  CREATED: 'Đã tạo thành công',
}

export const VALIDATION_MESSAGES = {
  REQUIRED: 'Trường này là bắt buộc',
  INVALID_EMAIL: 'Email không hợp lệ',
  INVALID_PHONE: 'Số điện thoại không hợp lệ',
  MIN_LENGTH: (min) => `Tối thiểu ${min} ký tự`,
  MAX_LENGTH: (max) => `Tối đa ${max} ký tự`,
  MIN_VALUE: (min) => `Giá trị phải lớn hơn hoặc bằng ${min}`,
  MAX_VALUE: (max) => `Giá trị phải nhỏ hơn hoặc bằng ${max}`,
  INVALID_NUMBER: 'Vui lòng nhập số hợp lệ',
}

