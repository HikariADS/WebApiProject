import { useEffect, useState, useMemo } from 'react'
import { productService } from '../api/productService'
import { productTypeService } from '../api/productTypeService'
import { useAuth } from '../contexts/AuthContext'
import { useToast } from '../contexts/ToastContext'
import { handleApiError, logError } from '../utils/errorHandler'
import { normalizeResponse } from '../utils/dataNormalizer'
import { ERROR_MESSAGES, SUCCESS_MESSAGES } from '../utils/constants'
import './TablePage.css'

const Products = () => {
  const [products, setProducts] = useState([])
  const [productTypes, setProductTypes] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [searchTerm, setSearchTerm] = useState('')
  const { user: currentUser } = useAuth()
  const { showToast } = useToast()
  const [showModal, setShowModal] = useState(false)
  const [editingProduct, setEditingProduct] = useState(null)
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    price: '',
    productTypeId: '',
  })
  const [formErrors, setFormErrors] = useState({})

  useEffect(() => {
    fetchProducts()
    fetchProductTypes()
  }, [])

  const fetchProducts = async () => {
    try {
      setLoading(true)
      const response = await productService.getAll()
      const normalized = normalizeResponse(response)
      setProducts(normalized.items || [])
      setError('')
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.FETCH_FAILED, 'Products.fetchProducts')
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  const fetchProductTypes = async () => {
    try {
      const data = await productTypeService.getAll()
      setProductTypes(Array.isArray(data) ? data : [])
    } catch (err) {
      logError(err, 'Products.fetchProductTypes')
      showToast('Không thể tải danh sách loại sản phẩm', 'error')
    }
  }

  const handleOpenModal = (product = null) => {
    if (product) {
      setEditingProduct(product)
      setFormData({
        name: product.name || product.Name || '',
        description: product.description || product.Description || '',
        price: product.price || product.Price || '',
        productTypeId: product.productTypeId || product.ProductTypeId || '',
      })
    } else {
      setEditingProduct(null)
      setFormData({
        name: '',
        description: '',
        price: '',
        productTypeId: '',
      })
    }
    setShowModal(true)
  }

  const handleCloseModal = () => {
    setShowModal(false)
    setEditingProduct(null)
    setFormErrors({})
  }

  const handleInputChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({
      ...prev,
      [name]: value
    }))
  }

  const handleSaveProduct = async () => {
    // Reset errors
    setFormErrors({})
    
    // Validation
    const errors = {}
    if (!formData.name.trim()) {
      errors.name = 'Vui lòng nhập tên sản phẩm'
    }
    if (formData.price && parseFloat(formData.price) < 0) {
      errors.price = 'Giá phải lớn hơn hoặc bằng 0'
    }
    
    if (Object.keys(errors).length > 0) {
      setFormErrors(errors)
      return
    }

    try {
      if (editingProduct) {
        // Update existing product
        const updateData = {
          id: editingProduct.id || editingProduct.Id,
          name: formData.name,
          description: formData.description,
          price: parseFloat(formData.price) || 0,
          productTypeId: parseInt(formData.productTypeId) || 0
        }
        await productService.update(updateData.id, updateData)
        showToast(SUCCESS_MESSAGES.UPDATED, 'success')
      } else {
        // Create new product
        const createData = {
          name: formData.name,
          description: formData.description,
          price: parseFloat(formData.price) || 0,
          productTypeId: parseInt(formData.productTypeId) || 0
        }
        await productService.create(createData)
        showToast(SUCCESS_MESSAGES.CREATED, 'success')
      }
      handleCloseModal()
      fetchProducts()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.SAVE_FAILED, 'Products.handleSaveProduct')
      showToast(errorMessage, 'error')
    }
  }

  const handleDeleteProduct = async (productId) => {
    if (!window.confirm('Bạn chắc chắn muốn xóa sản phẩm này?')) {
      return
    }

    try {
      await productService.delete(productId)
      showToast(SUCCESS_MESSAGES.DELETED, 'success')
      fetchProducts()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.DELETE_FAILED, 'Products.handleDeleteProduct')
      showToast(errorMessage, 'error')
    }
  }

  // Filter products based on search term
  const filteredProducts = useMemo(() => {
    if (!searchTerm.trim()) return products
    
    const term = searchTerm.toLowerCase()
    return products.filter(product => 
      (product.name || product.Name || '').toLowerCase().includes(term) ||
      (product.description || product.Description || '').toLowerCase().includes(term) ||
      (product.productTypeName || product.ProductTypeName || '').toLowerCase().includes(term)
    )
  }, [products, searchTerm])

  if (loading) {
    return <div className="loading">Đang tải...</div>
  }

  if (error) {
    return <div className="error">{error}</div>
  }

  // Kiểm tra quyền: chỉ Admin và Manager mới có thể thêm/sửa/xóa
  const canEdit = currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')

  return (
    <div className="table-page with-card-view">
      <div className="page-header">
        <h2>Danh sách sản phẩm</h2>
        {canEdit && (
          <button className="btn-primary" onClick={() => handleOpenModal()}>
            <span style={{ marginRight: '8px' }}>+</span>
            Thêm sản phẩm
          </button>
        )}
      </div>
      
      {/* Search Bar */}
      <div className="search-bar">
        <input
          type="text"
          placeholder="🔍 Tìm kiếm sản phẩm, mô tả, loại sản phẩm..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
        {searchTerm && (
          <button 
            className="search-clear" 
            onClick={() => setSearchTerm('')}
            title="Xóa tìm kiếm"
          >
            ×
          </button>
        )}
      </div>

      {/* Desktop Table View */}
      <div className="table-container">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Tên sản phẩm</th>
              <th>Mô tả</th>
              <th>Giá</th>
              <th>Loại sản phẩm</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {filteredProducts.length === 0 ? (
              <tr>
                <td colSpan="6" className="empty-state">
                  {searchTerm ? 'Không tìm thấy sản phẩm nào' : 'Chưa có sản phẩm nào'}
                </td>
              </tr>
            ) : (
              filteredProducts.map((product) => (
                <tr key={product.id || product.Id}>
                  <td>{product.id || product.Id}</td>
                  <td>{product.name || product.Name}</td>
                  <td>{product.description || product.Description || '-'}</td>
                  <td>{product.price || product.Price ? `${(product.price || product.Price).toLocaleString('vi-VN')}đ` : '-'}</td>
                  <td>{product.productTypeName || product.ProductTypeName || '-'}</td>
                  <td>
                    {canEdit ? (
                      <>
                        <button className="btn-edit" onClick={() => handleOpenModal(product)}>Sửa</button>
                        <button className="btn-delete" onClick={() => handleDeleteProduct(product.id || product.Id)}>Xóa</button>
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

      {/* Mobile Card View */}
      <div className="card-view">
        {filteredProducts.length === 0 ? (
          <div className="empty-state">
            {searchTerm ? 'Không tìm thấy sản phẩm nào' : 'Chưa có sản phẩm nào'}
          </div>
        ) : (
          filteredProducts.map((product) => (
            <div key={product.id || product.Id} className="card-item">
              <div className="card-item-header">
                <div className="card-item-title">{product.name || product.Name}</div>
                {canEdit && (
                  <div className="card-item-actions">
                    <button 
                      className="btn-edit" 
                      onClick={() => handleOpenModal(product)}
                      style={{ fontSize: '0.75rem', padding: '4px 8px' }}
                    >
                      Sửa
                    </button>
                    <button 
                      className="btn-delete" 
                      onClick={() => handleDeleteProduct(product.id || product.Id)}
                      style={{ fontSize: '0.75rem', padding: '4px 8px' }}
                    >
                      Xóa
                    </button>
                  </div>
                )}
              </div>
              <div className="card-item-body">
                <div className="card-item-row">
                  <span className="card-item-label">ID:</span>
                  <span className="card-item-value">{product.id || product.Id}</span>
                </div>
                <div className="card-item-row">
                  <span className="card-item-label">Mô tả:</span>
                  <span className="card-item-value">
                    {(product.description || product.Description || '-').substring(0, 50)}
                    {(product.description || product.Description || '').length > 50 ? '...' : ''}
                  </span>
                </div>
                <div className="card-item-row">
                  <span className="card-item-label">Giá:</span>
                  <span className="card-item-value">
                    {product.price || product.Price 
                      ? `${(product.price || product.Price).toLocaleString('vi-VN')}đ` 
                      : '-'}
                  </span>
                </div>
                <div className="card-item-row">
                  <span className="card-item-label">Loại sản phẩm:</span>
                  <span className="card-item-value">
                    {product.productTypeName || product.ProductTypeName || '-'}
                  </span>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {/* Modal for Add/Edit Product */}
      {showModal && (
        <div className="modal-overlay" onClick={handleCloseModal}>
          <div className="modal-content" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <h3>{editingProduct ? 'Sửa sản phẩm' : 'Thêm sản phẩm mới'}</h3>
              <button className="btn-close" onClick={handleCloseModal}>&times;</button>
            </div>
            <div className="modal-body">
              <div className="form-group">
                <label>Tên sản phẩm *</label>
                <input
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  placeholder="Nhập tên sản phẩm"
                  className={formErrors.name ? 'input-error' : ''}
                />
                {formErrors.name && <span className="error-text">{formErrors.name}</span>}
              </div>
              <div className="form-group">
                <label>Mô tả</label>
                <textarea
                  name="description"
                  value={formData.description}
                  onChange={handleInputChange}
                  placeholder="Nhập mô tả sản phẩm"
                  rows="4"
                />
              </div>
              <div className="form-group">
                <label>Giá</label>
                <input
                  type="number"
                  name="price"
                  value={formData.price}
                  onChange={handleInputChange}
                  placeholder="Nhập giá sản phẩm"
                  min="0"
                  step="0.01"
                  className={formErrors.price ? 'input-error' : ''}
                />
                {formErrors.price && <span className="error-text">{formErrors.price}</span>}
              </div>
              <div className="form-group">
                <label>Loại sản phẩm</label>
                <select
                  name="productTypeId"
                  value={formData.productTypeId}
                  onChange={handleInputChange}
                >
                  <option value="">-- Chọn loại sản phẩm --</option>
                  {productTypes.map((type) => (
                    <option key={type.id || type.Id} value={type.id || type.Id}>
                      {type.name || type.Name}
                    </option>
                  ))}
                </select>
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn-secondary" onClick={handleCloseModal}>Hủy</button>
              <button className="btn-primary" onClick={handleSaveProduct}>
                {editingProduct ? 'Cập nhật' : 'Thêm mới'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

export default Products

