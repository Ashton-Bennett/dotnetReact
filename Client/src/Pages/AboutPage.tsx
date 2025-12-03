import { useNavigate } from "react-router-dom";

const AboutPage = () => {
  const navigate = useNavigate();
  const goBack = () => {
    navigate(-1);
  };
  return (
    <div className="flex-1">
      <section>
        <h1>About Skyline Software</h1>
        <p>
          Building modern, scalable software solutions that help businesses
          grow.
        </p>
      </section>

      <section>
        <h2>Who We Are</h2>
        <p>
          Skyline Software is a team of passionate engineers, designers, and
          problem-solvers based in Walla Walla, WA. We specialize in crafting
          web and mobile applications that simplify complex workflows and
          empower organizations to work smarter.
        </p>
      </section>

      <section>
        <h2>Our Mission</h2>
        <p>
          Our mission is to create elegant, reliable software that enhances
          productivity and drives innovation. We believe in combining clean
          design with modern technologies to deliver experiences users love.
        </p>
      </section>

      <section>
        <h2>Our Values</h2>
        <ul>
          <li>💡 Innovation — Always explore new ideas and technologies.</li>
          <li>
            🤝 Collaboration — Build meaningful partnerships with our clients.
          </li>
          <li>⚙️ Quality — Write clean, maintainable, and tested code.</li>
          <li>🌍 Impact — Create tools that make a real difference.</li>
        </ul>
      </section>
      <button onClick={goBack}>Go Back</button>
    </div>
  );
};

export default AboutPage;
