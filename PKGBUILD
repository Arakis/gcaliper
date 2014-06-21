# Maintainer: Sebastian Loncar <sebastian.loncar@gmail.com>

pkgname=gcaliper
pkgver=1.0.2
pkgrel=1
pkgdesc="Professional screen caliper and ruler with arbitrary rotation"
url="https://github.com/Arakis/gcaliper"
license=("BSD")
depends=("mono", "gtk-sharp-2")
makedepends=("git")
provides=("gcaliper")
conflicts=("gcaliper")
source=("git://github.com/Arakis/gcaliper.git")
arch=(any)
md5sums=("SKIP")

#pkgver() {
#  cd exec-or-activate-application
#  git rev-list HEAD --count
#}

build() {
  cd gcaliper
  make DESTDIR="$pkgdir"
}

package() {
  cd gcaliper
  make DESTDIR="$pkgdir" install
}
