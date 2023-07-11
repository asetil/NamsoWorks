import BaseService from './BaseService';

class HomeService extends BaseService {
    getSliderItems() {
        return super.getData("/Common/SliderItems");
    }
}

export default HomeService = new HomeService();