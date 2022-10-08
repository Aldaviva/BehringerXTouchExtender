namespace Tests;

public class BehringerXTouchExtenderFactoryTest {

    [Fact]
    public void Relative() {
        IRelativeBehringerXTouchExtender actual = BehringerXTouchExtenderFactory.CreateWithRelativeMode();
        actual.Should().BeOfType<RelativeBehringerXTouchExtender>();
    }

    [Fact]
    public void Absolute() {
        IAbsoluteBehringerXTouchExtender actual = BehringerXTouchExtenderFactory.CreateWithAbsoluteMode();
        actual.Should().BeOfType<AbsoluteBehringerXTouchExtender>();
    }

}