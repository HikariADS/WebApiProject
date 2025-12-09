import { useEffect, useState } from 'react'
import { productService } from '../api/productService'
import { storageService } from '../api/storageService'
import { productTypeService } from '../api/productTypeService'
import { storageTypeService } from '../api/storageTypeService'
import './Dashboard.css'

const Dashboard = () => {
  const [stats, setStats] = useState({
    totalProducts: 0,
    totalStorage: 0,
    totalProductTypes: 0,
    totalStorageTypes: 0,
    totalQuantity: 0,
    loading: true,
  })

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const [productsRes, storageRes, productTypesRes, storageTypesRes] = await Promise.all([
          productService.getAll({ pageSize: 1 }),
          storageService.getAll(),
          productTypeService.getAll(),
          storageTypeService.getAll(),
        ])

        // Products trả về PageResult
        const totalProducts = productsRes.totalItems || productsRes.TotalItems || 0
        
        // Storage trả về array trực tiếp
        const storages = Array.isArray(storageRes) ? storageRes : []
        const totalStorage = storages.length
        const totalQuantity = storages.reduce((sum, s) => sum + (s.quantity || s.Quantity || 0), 0)
        
        // ProductTypes và StorageTypes trả về array
        const productTypes = Array.isArray(productTypesRes) ? productTypesRes : []
        const storageTypes = Array.isArray(storageTypesRes) ? storageTypesRes : []

        setStats({
          totalProducts,
          totalStorage,
          totalProductTypes: productTypes.length,
          totalStorageTypes: storageTypes.length,
          totalQuantity,
          loading: false,
        })
      } catch (error) {
        console.error('Error fetching stats:', error)
        setStats((prev) => ({ ...prev, loading: false }))
      }
    }

    fetchStats()
  }, [])

  if (stats.loading) {
    return <div className="loading">Đang tải...</div>
  }

  return (
    <div className="dashboard">
      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-icon">📦</div>
          <div className="stat-content">
            <h3>Tổng sản phẩm</h3>
            <p className="stat-value">{stats.totalProducts}</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon">🏷️</div>
          <div className="stat-content">
            <h3>Loại sản phẩm</h3>
            <p className="stat-value">{stats.totalProductTypes}</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon">🏪</div>
          <div className="stat-content">
            <h3>Tổng kho</h3>
            <p className="stat-value">{stats.totalStorage}</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon">📋</div>
          <div className="stat-content">
            <h3>Loại kho</h3>
            <p className="stat-value">{stats.totalStorageTypes}</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon">📊</div>
          <div className="stat-content">
            <h3>Tổng số lượng</h3>
            <p className="stat-value">{stats.totalQuantity.toLocaleString('vi-VN')}</p>
          </div>
        </div>
      </div>
      <div className="welcome-section">
        <h2>Chào mừng đến với hệ thống quản lý kho</h2>
        <p>Sử dụng menu bên trái để điều hướng đến các chức năng quản lý.</p>
        <div className="quick-info">
          <p><strong>Hệ thống quản lý kho</strong> giúp bạn quản lý sản phẩm, kho hàng và người dùng một cách hiệu quả.</p>
        </div>
      </div>
    </div>
  )
}

export default Dashboard

