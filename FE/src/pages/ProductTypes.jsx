import { useEffect, useState } from 'react'
import { productTypeService } from '../api/productTypeService'
import './TablePage.css'
import './ProductTypes.css'

const ProductTypes = () => {
  const [productTypes, setProductTypes] = useState([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [formData, setFormData] = useState({ name: '', description: '' })
  const [editingId, setEditingId] = useState(null)
  const [error, setError] = useState('')

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
      setError('Không thể tải danh sách loại sản phẩm')
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  const handleOpenModal = (productType = null) => {
    if (productType) {
      setFormData({ name: productType.name || productType.Name || '', description: productType.description || productType.Description || '' })
      setEditingId(productType.id || productType.Id)
    } else {
      setFormData({ name: '', description: '' })
      setEditingId(null)
    }
    setShowModal(true)
    setError('')
  }

  const handleCloseModal = () => {
    setShowModal(false)
    setFormData({ name: '', description: '' })
    setEditingId(null)
    setError('')
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')

    try {
      if (editingId) {
        await productTypeService.update(editingId, formData)
      } else {
        await productTypeService.create(formData)
      }
      handleCloseModal()
      fetchProductTypes()
    } catch (err) {
      setError(err.response?.data?.message || 'Có lỗi xảy ra')
    }
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Bạn có chắc chắn muốn xóa loại sản phẩm này?')) {
      return
    }

    try {
      await productTypeService.delete(id)
      fetchProductTypes()
    } catch (err) {
      alert('Không thể xóa loại sản phẩm')
      console.error(err)
    }
  }

  if (loading) {
    return <div className="loading">Đang tải...</div>
  }

  return (
    <div className="table-page">
      <div className="page-header">
        <h2>Danh sách loại sản phẩm</h2>
        <button className="btn-primary" onClick={() => handleOpenModal()}>
          Thêm loại sản phẩm
        </button>
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
            {productTypes.length === 0 ? (
              <tr>
                <td colSpan="4" className="empty-state">
                  Chưa có dữ liệu
                </td>
              </tr>
            ) : (
              productTypes.map((type) => (
                <tr key={type.id || type.Id}>
                  <td>{type.id || type.Id}</td>
                  <td>{type.name || type.Name}</td>
                  <td>{type.description || type.Description || '-'}</td>
                  <td>
                    <button className="btn-edit" onClick={() => handleOpenModal(type)}>
                      Sửa
                    </button>
                    <button className="btn-delete" onClick={() => handleDelete(type.id || type.Id)}>
                      Xóa
                    </button>
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
            <form onSubmit={handleSubmit}>
              {error && <div className="error-message">{error}</div>}
              <div className="form-group">
                <label htmlFor="name">Tên loại *</label>
                <input
                  type="text"
                  id="name"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  required
                  maxLength={100}
                  placeholder="Nhập tên loại sản phẩm"
                />
              </div>
              <div className="form-group">
                <label htmlFor="description">Mô tả *</label>
                <textarea
                  id="description"
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  required
                  maxLength={225}
                  rows={4}
                  placeholder="Nhập mô tả"
                />
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

