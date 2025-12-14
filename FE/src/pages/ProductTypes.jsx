import { useEffect, useState, useMemo } from 'react'
import { productTypeService } from '../api/productTypeService'
import { useAuth } from '../contexts/AuthContext'
import { useToast } from '../contexts/ToastContext'
import { handleApiError } from '../utils/errorHandler'
import { ERROR_MESSAGES, SUCCESS_MESSAGES, VALIDATION_MESSAGES } from '../utils/constants'
import './TablePage.css'
import './ProductTypes.css'

const ProductTypes = () => {
  const [productTypes, setProductTypes] = useState([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [formData, setFormData] = useState({ name: '', description: '' })
  const [formErrors, setFormErrors] = useState({})
  const [searchTerm, setSearchTerm] = useState('')
  const [editingId, setEditingId] = useState(null)
  const [error, setError] = useState('')
  const { user: currentUser } = useAuth()
  const { showToast } = useToast()

  useEffect(() => {
    fetchProductTypes()
  }, [])

  const fetchProductTypes = async () => {
    try {
      setLoading(true)
      const data = await productTypeService.getAll()
      setProductTypes(Array.isArray(data) ? data : [])
      setError('')
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.FETCH_FAILED, 'ProductTypes.fetchProductTypes')
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  const handleOpenModal = (productType = null) => {
    if (productType) {
      setFormData({ 
        name: productType.name || productType.Name || '', 
        description: productType.description || productType.Description || '' 
      })
      setEditingId(productType.id || productType.Id)
    } else {
      setFormData({ name: '', description: '' })
      setEditingId(null)
    }
    setFormErrors({})
    setShowModal(true)
    setError('')
  }

  const handleCloseModal = () => {
    setShowModal(false)
    setFormData({ name: '', description: '' })
    setFormErrors({})
    setEditingId(null)
    setError('')
  }

  const validateForm = () => {
    const errors = {}
    
    if (!formData.name.trim()) {
      errors.name = VALIDATION_MESSAGES.REQUIRED
    } else if (formData.name.trim().length < 2) {
      errors.name = VALIDATION_MESSAGES.MIN_LENGTH(2)
    } else if (formData.name.trim().length > 100) {
      errors.name = VALIDATION_MESSAGES.MAX_LENGTH(100)
    }
    
    if (!formData.description.trim()) {
      errors.description = VALIDATION_MESSAGES.REQUIRED
    } else if (formData.description.trim().length < 5) {
      errors.description = VALIDATION_MESSAGES.MIN_LENGTH(5)
    } else if (formData.description.trim().length > 225) {
      errors.description = VALIDATION_MESSAGES.MAX_LENGTH(225)
    }
    
    return errors
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setFormErrors({})

    // Validate form
    const validationErrors = validateForm()
    if (Object.keys(validationErrors).length > 0) {
      setFormErrors(validationErrors)
      showToast('Vui lòng kiểm tra lại thông tin đã nhập', 'error')
      return
    }

    try {
      if (editingId) {
        await productTypeService.update(editingId, formData)
        showToast(SUCCESS_MESSAGES.UPDATED, 'success')
      } else {
        await productTypeService.create(formData)
        showToast(SUCCESS_MESSAGES.CREATED, 'success')
      }
      handleCloseModal()
      fetchProductTypes()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.SAVE_FAILED, 'ProductTypes.handleSubmit')
      setError(errorMessage)
      showToast(errorMessage, 'error')
    }
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Bạn có chắc chắn muốn xóa loại sản phẩm này?')) {
      return
    }

    try {
      await productTypeService.delete(id)
      showToast(SUCCESS_MESSAGES.DELETED, 'success')
      fetchProductTypes()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.DELETE_FAILED, 'ProductTypes.handleDelete')
      showToast(errorMessage, 'error')
    }
  }

  // Filter product types based on search term
  const filteredProductTypes = useMemo(() => {
    if (!searchTerm.trim()) return productTypes
    
    const term = searchTerm.toLowerCase()
    return productTypes.filter(type => 
      (type.name || type.Name || '').toLowerCase().includes(term) ||
      (type.description || type.Description || '').toLowerCase().includes(term)
    )
  }, [productTypes, searchTerm])

  if (loading) {
    return <div className="loading">Đang tải...</div>
  }

  // Kiểm tra quyền: chỉ Admin và Manager mới có thể thêm/sửa/xóa
  const canEdit = currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')

  return (
    <div className="table-page">
      <div className="page-header">
        <h2>Danh sách loại sản phẩm</h2>
        <div style={{ display: 'flex', gap: '10px', alignItems: 'center', flexWrap: 'wrap' }}>
          <div className="search-bar" style={{ flex: '1', minWidth: '200px', maxWidth: '400px' }}>
            <input
              type="text"
              placeholder="Tìm kiếm theo tên, mô tả..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              style={{
                width: '100%',
                padding: '8px 12px',
                border: '1px solid #ddd',
                borderRadius: '4px',
                fontSize: '0.9rem'
              }}
            />
          </div>
          {canEdit && (
            <button className="btn-primary" onClick={() => handleOpenModal()}>
              Thêm loại sản phẩm
            </button>
          )}
        </div>
      </div>
      {error && !showModal && <div className="error">{error}</div>}
      <div className="table-container">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Tên loại</th>
              <th>Mô tả</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {filteredProductTypes.length === 0 ? (
              <tr>
                <td colSpan="4" className="empty-state">
                  {searchTerm ? 'Không tìm thấy kết quả' : 'Chưa có dữ liệu'}
                </td>
              </tr>
            ) : (
              filteredProductTypes.map((type) => (
                <tr key={type.id || type.Id}>
                  <td>{type.id || type.Id}</td>
                  <td>{type.name || type.Name}</td>
                  <td>{type.description || type.Description || '-'}</td>
                  <td>
                    {canEdit ? (
                      <>
                        <button className="btn-edit" onClick={() => handleOpenModal(type)}>
                          Sửa
                        </button>
                        <button className="btn-delete" onClick={() => handleDelete(type.id || type.Id)}>
                          Xóa
                        </button>
                      </>
                    ) : (
                      <span style={{ color: '#999', fontSize: '0.9rem' }}>Chỉ xem</span>
                    )}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {showModal && (
        <div className="modal-overlay" onClick={handleCloseModal}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>{editingId ? 'Sửa loại sản phẩm' : 'Thêm loại sản phẩm'}</h3>
              <button className="modal-close" onClick={handleCloseModal}>×</button>
            </div>
            <form onSubmit={handleSubmit} noValidate>
              {error && <div className="error-message">{error}</div>}
              <div className="form-group">
                <label htmlFor="name">Tên loại *</label>
                <input
                  type="text"
                  id="name"
                  value={formData.name}
                  onChange={(e) => {
                    setFormData({ ...formData, name: e.target.value })
                    if (formErrors.name) {
                      setFormErrors({ ...formErrors, name: '' })
                    }
                  }}
                  maxLength={100}
                  placeholder="Nhập tên loại sản phẩm (ví dụ: Điện tử, Thực phẩm, Quần áo)"
                  className={formErrors.name ? 'error' : ''}
                />
                {formErrors.name && <span className="field-error">{formErrors.name}</span>}
                <small style={{ color: '#666', fontSize: '0.85rem', marginTop: '4px', display: 'block' }}>
                  {formData.name.length}/100 ký tự
                </small>
              </div>
              <div className="form-group">
                <label htmlFor="description">Mô tả *</label>
                <textarea
                  id="description"
                  value={formData.description}
                  onChange={(e) => {
                    setFormData({ ...formData, description: e.target.value })
                    if (formErrors.description) {
                      setFormErrors({ ...formErrors, description: '' })
                    }
                  }}
                  maxLength={225}
                  rows={4}
                  placeholder="Nhập mô tả chi tiết về loại sản phẩm"
                  className={formErrors.description ? 'error' : ''}
                />
                {formErrors.description && <span className="field-error">{formErrors.description}</span>}
                <small style={{ color: '#666', fontSize: '0.85rem', marginTop: '4px', display: 'block' }}>
                  {formData.description.length}/225 ký tự
                </small>
              </div>
              <div className="modal-actions">
                <button type="button" className="btn-cancel" onClick={handleCloseModal}>
                  Hủy
                </button>
                <button type="submit" className="btn-primary">
                  {editingId ? 'Cập nhật' : 'Thêm mới'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}

export default ProductTypes

