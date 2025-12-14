import { useEffect, useState, useMemo } from 'react'
import { useAuth } from '../contexts/AuthContext'
import { useToast } from '../contexts/ToastContext'
import { newsService } from '../api/newsService'
import { handleApiError, logError } from '../utils/errorHandler'
import { normalizeArray } from '../utils/dataNormalizer'
import { ERROR_MESSAGES, SUCCESS_MESSAGES, VALIDATION_MESSAGES } from '../utils/constants'
import './News.css'
import './TablePage.css'

const News = () => {
  const { user: currentUser } = useAuth()
  const { showToast } = useToast()
  const [news, setNews] = useState([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [formData, setFormData] = useState({
    title: '',
    content: '',
    category: 'Thông báo'
  })
  const [formErrors, setFormErrors] = useState({})
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedCategory, setSelectedCategory] = useState('Tất cả')
  const [editingId, setEditingId] = useState(null)

  const categories = ['Tất cả', 'Thông báo', 'Cập nhật', 'Hướng dẫn', 'Khác']

  useEffect(() => {
    fetchNews()
  }, [])

  const fetchNews = async () => {
    try {
      setLoading(true)
      const data = await newsService.getAll()
      setNews(normalizeArray(data))
    } catch (err) {
      logError(err, 'News.fetchNews')
      // Không hiển thị toast cho public page để tránh làm phiền người dùng chưa login
      if (currentUser) {
        showToast('Không thể tải danh sách tin tức', 'error')
      }
    } finally {
      setLoading(false)
    }
  }

  const handleOpenModal = (item = null) => {
    if (item) {
      setFormData({
        title: item.title || '',
        content: item.content || '',
        category: item.category || 'Thông báo'
      })
      setEditingId(item.id)
    } else {
      setFormData({
        title: '',
        content: '',
        category: 'Thông báo'
      })
      setEditingId(null)
    }
    setFormErrors({})
    setShowModal(true)
  }

  const handleCloseModal = () => {
    setShowModal(false)
    setFormData({
      title: '',
      content: '',
      category: 'Thông báo'
    })
    setFormErrors({})
    setEditingId(null)
  }

  const validateForm = () => {
    const errors = {}
    
    if (!formData.title.trim()) {
      errors.title = VALIDATION_MESSAGES.REQUIRED
    } else if (formData.title.trim().length < 5) {
      errors.title = VALIDATION_MESSAGES.MIN_LENGTH(5)
    } else if (formData.title.trim().length > 200) {
      errors.title = VALIDATION_MESSAGES.MAX_LENGTH(200)
    }
    
    if (!formData.content.trim()) {
      errors.content = VALIDATION_MESSAGES.REQUIRED
    } else if (formData.content.trim().length < 10) {
      errors.content = VALIDATION_MESSAGES.MIN_LENGTH(10)
    } else if (formData.content.trim().length > 2000) {
      errors.content = VALIDATION_MESSAGES.MAX_LENGTH(2000)
    }
    
    return errors
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setFormErrors({})

    const validationErrors = validateForm()
    if (Object.keys(validationErrors).length > 0) {
      setFormErrors(validationErrors)
      showToast('Vui lòng kiểm tra lại thông tin đã nhập', 'error')
      return
    }

    try {
      if (editingId) {
        await newsService.update(editingId, { ...formData, id: editingId })
        showToast(SUCCESS_MESSAGES.UPDATED, 'success')
      } else {
        await newsService.create(formData)
        showToast(SUCCESS_MESSAGES.CREATED, 'success')
      }
      
      handleCloseModal()
      fetchNews()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.SAVE_FAILED, 'News.handleSubmit')
      showToast(errorMessage, 'error')
    }
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Bạn có chắc chắn muốn xóa tin tức này?')) {
      return
    }

    try {
      await newsService.delete(id)
      showToast(SUCCESS_MESSAGES.DELETED, 'success')
      fetchNews()
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.DELETE_FAILED, 'News.handleDelete')
      showToast(errorMessage, 'error')
    }
  }

  // Filter news
  const filteredNews = useMemo(() => {
    let filtered = news

    // Filter by category
    if (selectedCategory !== 'Tất cả') {
      filtered = filtered.filter(item => item.category === selectedCategory)
    }

    // Filter by search term
    if (searchTerm.trim()) {
      const term = searchTerm.toLowerCase()
      filtered = filtered.filter(item =>
        (item.title || '').toLowerCase().includes(term) ||
        (item.content || '').toLowerCase().includes(term) ||
        (item.authorName || '').toLowerCase().includes(term)
      )
    }

    // Sort by date (newest first)
    return filtered.sort((a, b) => {
      const dateA = new Date(a.createdAt || a.updatedAt || 0)
      const dateB = new Date(b.createdAt || b.updatedAt || 0)
      return dateB - dateA
    })
  }, [news, searchTerm, selectedCategory])

  if (loading) {
    return <div className="loading">Đang tải...</div>
  }

  const canEdit = currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')

  return (
    <div className="news-page">
      <div className="news-header">
        <div>
          <h1>Tin tức & Thông báo</h1>
          <p className="news-subtitle">Cập nhật mới nhất từ hệ thống</p>
        </div>
        {canEdit && (
          <button className="btn-primary" onClick={() => handleOpenModal()}>
            + Thêm tin tức
          </button>
        )}
      </div>

      <div className="news-filters">
        <div className="search-bar">
          <input
            type="text"
            placeholder="Tìm kiếm tin tức..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
        <div className="category-filter">
          {categories.map(cat => (
            <button
              key={cat}
              className={`category-btn ${selectedCategory === cat ? 'active' : ''}`}
              onClick={() => setSelectedCategory(cat)}
            >
              {cat}
            </button>
          ))}
        </div>
      </div>

      {filteredNews.length === 0 ? (
        <div className="empty-state">
          {searchTerm || selectedCategory !== 'Tất cả' 
            ? 'Không tìm thấy tin tức nào' 
            : 'Chưa có tin tức nào'}
        </div>
      ) : (
        <div className="news-grid">
          {filteredNews.map((item) => (
            <div key={item.id} className="news-card">
              <div className="news-card-header">
                <div className="news-category">{item.category}</div>
                {canEdit && (
                  <div className="news-actions">
                    <button 
                      className="btn-edit-small" 
                      onClick={() => handleOpenModal(item)}
                      title="Sửa"
                    >
                      ✏️
                    </button>
                    <button 
                      className="btn-delete-small" 
                      onClick={() => handleDelete(item.id)}
                      title="Xóa"
                    >
                      🗑️
                    </button>
                  </div>
                )}
              </div>
              <div className="news-card-body">
                <h3 className="news-title">{item.title}</h3>
                <p className="news-content">
                  {(item.content || '').length > 150 
                    ? `${(item.content || '').substring(0, 150)}...` 
                    : (item.content || '')}
                </p>
              </div>
              <div className="news-card-footer">
                <div className="news-author">
                  <span>👤 {item.authorName || 'Unknown'}</span>
                </div>
                <div className="news-date">
                  📅 {new Date(item.createdAt || item.updatedAt || Date.now()).toLocaleDateString('vi-VN', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric'
                  })}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {showModal && (
        <div className="modal-overlay" onClick={handleCloseModal}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>{editingId ? 'Sửa tin tức' : 'Thêm tin tức'}</h3>
              <button className="modal-close" onClick={handleCloseModal}>×</button>
            </div>
            <form onSubmit={handleSubmit} noValidate>
              <div className="form-group">
                <label htmlFor="category">Danh mục *</label>
                <select
                  id="category"
                  value={formData.category}
                  onChange={(e) => setFormData({ ...formData, category: e.target.value })}
                >
                  <option value="Thông báo">Thông báo</option>
                  <option value="Cập nhật">Cập nhật</option>
                  <option value="Hướng dẫn">Hướng dẫn</option>
                  <option value="Khác">Khác</option>
                </select>
              </div>
              <div className="form-group">
                <label htmlFor="title">Tiêu đề *</label>
                <input
                  type="text"
                  id="title"
                  value={formData.title}
                  onChange={(e) => {
                    setFormData({ ...formData, title: e.target.value })
                    if (formErrors.title) {
                      setFormErrors({ ...formErrors, title: '' })
                    }
                  }}
                  maxLength={200}
                  placeholder="Nhập tiêu đề tin tức"
                  className={formErrors.title ? 'error' : ''}
                />
                {formErrors.title && <span className="field-error">{formErrors.title}</span>}
                <small style={{ color: '#666', fontSize: '0.85rem', marginTop: '4px', display: 'block' }}>
                  {formData.title.length}/200 ký tự
                </small>
              </div>
              <div className="form-group">
                <label htmlFor="content">Nội dung *</label>
                <textarea
                  id="content"
                  value={formData.content}
                  onChange={(e) => {
                    setFormData({ ...formData, content: e.target.value })
                    if (formErrors.content) {
                      setFormErrors({ ...formErrors, content: '' })
                    }
                  }}
                  maxLength={2000}
                  rows={8}
                  placeholder="Nhập nội dung tin tức"
                  className={formErrors.content ? 'error' : ''}
                />
                {formErrors.content && <span className="field-error">{formErrors.content}</span>}
                <small style={{ color: '#666', fontSize: '0.85rem', marginTop: '4px', display: 'block' }}>
                  {formData.content.length}/2000 ký tự
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

export default News

